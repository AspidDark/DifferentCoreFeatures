using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using ABSService;
using ABSUploadClient.Dto;
using ABSUploadClient.Entity.EntityModel;
using ABSUploadClient.Extensions;
using ABSUploadClient.Options;
using Newtonsoft.Json;
using Serilog;
using UploadClient.Models.AbsDTO;

namespace ABSUploadClient.Services
{
	public class PaymentSendService
	{
		private readonly PaymentOrdersService paymentOrdersService;
		private readonly AbsOptions absOptions;

		public PaymentSendService(PaymentOrdersService paymentOrdersService, AbsOptions options)
		{
			this.paymentOrdersService = paymentOrdersService;
			this.absOptions = options;
		}

		public async Task SendPaymentOrders(IFileService fileService, string moduleBrief,
			string authData, ILogger logger)
		{
			// список платежей для загрузки в АБС
			var paymentOrders = fileService.GetPaymentOrders();
			logger.Information("Парсинг выполнен успешно");

			// оставить только новые
			var filteredpayments = this.paymentOrdersService
				.FilterExistingOrders(paymentOrders)
				.ToArray();

			int N = filteredpayments.Length;
			logger.Information($"Всего записей {paymentOrders.Count()}, новых {N}");

			// установка статуса обработанных
			foreach (var sentOrder in paymentOrders.Except(filteredpayments))
				fileService.SetBindingResult(sentOrder, "Обработано ранее", string.Empty);

			int maxBlockSize = this.absOptions.MaxBlockSize;

			for (int i = 0; i < N; i += maxBlockSize)
			{
				int len = Math.Min(N - i, maxBlockSize);
				var ordersChunk = new PaymentOrder[len];

				for (int j = 0, k = i; j < len; j++, k++)
					ordersChunk[j] = filteredpayments[k];

				var massInsertPayments =
					this.GetPaymentsForMassInsert(ordersChunk, moduleBrief);

				// отправка платежей в буфер необработанных платежей
				logger.Information($"Загрузка платежей с {i + 1} по {i + len} в буфер необработанных платежей");
				var response = await this.UploadPaymentsToBufferAsync(massInsertPayments);

				var uploadResult = response.DsPaymentBufferMassInsertRes;

				// обработка результата отправки платежей
				var paymentBindingMap =
					await this.ProcessUploadPaymentsResults(ordersChunk, uploadResult, authData, fileService, logger);

				// получение списка пар (Id загруженной платёжки, Id кредитного договора)
				logger.Information("Получение пар (Id загруженной платежки - Id кредитного договора)");
				var paymentsToBind = await this.GetPaymentsToBind(ordersChunk, massInsertPayments,
					uploadResult, fileService, logger);

				foreach (var item in paymentsToBind)
					item.PaymentOrderId = paymentBindingMap
						.Where(x => x.PaymentOrder == item.PaymentOrder)
						.Select(y => y.PaymentOrderId)
						.FirstOrDefault();

				// привязка платежей к кредитным договорам
				logger.Information("Привязка платежей с указанным номером и найденным идентификатором договора");
				await this.BindPaymentsToCreditContractsAsync(paymentsToBind, fileService, logger);

				logger.Information($"Обработка результатов c {i + 1} по {i + len} завершена");
			}

			fileService.Save(paymentOrders);
		}

		private TPaymentListTypeForDSPaymentBufferMassInsert[] GetPaymentsForMassInsert(
			IEnumerable<PaymentOrder> paymentOrders, string moduleBrief)
		{
			return paymentOrders
				.Select((payment, i) => new TPaymentListTypeForDSPaymentBufferMassInsert
				{
					GUID = Guid.NewGuid().ToLong(),
					ModuleBrief = moduleBrief,
					Date = payment.IncomeDate,
					Amount = payment.Amount,
					Comment = payment.Description,
					Number = payment.Number,
					PayerName = payment.PayerName,
					PaymentKind = 3,
					LinkID = i // ВАЖНО: обработка ошибок будет ссылаться на этот порядковый номер.
										 // Связь TPaymentListTypeForDSPaymentBufferMassInsert[] и PaymentOrder[] - по порядковому номеру i
				})
				.ToArray();
		}

		/// <summary>
		/// Загрузка платёжек в буфер необработанных платежей: Бэк-офис/Кредиты/Обработка платежей
		/// </summary>
		/// <param name="payments"></param>
		/// <returns></returns>
		private async Task<dsPaymentBufferMassInsertResponse> UploadPaymentsToBufferAsync(TPaymentListTypeForDSPaymentBufferMassInsert[] payments)
		{
			var client = this.GetServiceClient();
			var result = await client.dsPaymentBufferMassInsertAsync(payments);
			await client.CloseAsync();
			return result;
		}

		/// <summary>
		/// Обработка результатов загрузки платежей в реестр неразнесенных платежей Бэк-офис/Кредиты/Обработка платежей
		/// </summary>
		private async Task<List<PaymentBindingMap>> ProcessUploadPaymentsResults(IReadOnlyList<PaymentOrder> paymentOrders,
			DsPaymentBufferMassInsertRes uploadResult, string authData, IFileService fileService, ILogger logger)
		{
			var absSendError = uploadResult
				.NotificationList?
				.Select(a => new AbsSentResultDto { LinkId = a.LinkID, ErrorMessage = a.NTFMessage });

			var paymentBindingMap = await this.paymentOrdersService
				.SaveAbsSentResult(paymentOrders, absSendError, authData);

			foreach (var order in paymentOrders)
				fileService.SetBindingResult(order, "Загружен в буфер необработанных платежей", string.Empty);

			if (uploadResult.Status == "OK")
				logger.Information("Загрузка платежей завершена");
			else
			{
				logger.Information("--- Ошибки загрузки в буфер необработанных платежей ---");
				foreach (var res in uploadResult.NotificationList)
				{
					int ind = (int)res.LinkID;
					string str = JsonConvert
						.SerializeObject(paymentOrders[ind], Formatting.Indented);
					logger.Information("Ошибка: {0} Платеж: {1}", res.NTFMessage, str);

					fileService.SetBindingResult(paymentOrders[ind],
						"Ошибка загрузки в буфер необработанных платежей: " + res.NTFMessage, string.Empty);
				}
				logger.Information("-------------------------------------------------------");
			}
			return paymentBindingMap;
		}

		/// <summary>
		/// Получить список пар: (Id загруженной платёжки, Id кредитного договора) на основе списка платежей,
		/// для которых заполнено поле НомерКредитногоДоговора
		/// </summary>
		private async Task<IEnumerable<PaymentOrderLinks>> GetPaymentsToBind(
			PaymentOrder[] paymentFromExcel,
			TPaymentListTypeForDSPaymentBufferMassInsert[] massInsertPayments,
			DsPaymentBufferMassInsertRes uploadResult, IFileService fileService, ILogger logger)
		{
			var errorLinks = uploadResult?
				.NotificationList?
				.ToDictionary(x => x.LinkID, x => x.LinkID) ?? new Dictionary<long, long>();

			var paymentsToBind = paymentFromExcel
				.Select((p, i) => (p.CreditContractNumber, massInsertPayments[i].GUID, massInsertPayments[i].LinkID, PaymentOrder: p))
				.Where(x => !string.IsNullOrEmpty(x.CreditContractNumber))
				.Where(x => !errorLinks.ContainsKey(x.LinkID))
				.Select(x => (PaymentGuid: x.GUID, x.CreditContractNumber, x.PaymentOrder))
				.ToList();

			if (!paymentsToBind.Any())
			{
				logger.Information($"Список платежек с указанными номерами договоров пуст");
				return Enumerable.Empty<PaymentOrderLinks>();
			}

			var paymentIds = await this.GetPaymentIdsAsync(paymentsToBind.Select(x => x.PaymentGuid), logger);
			var creditContractIds =
				await this.GetCreditContractIdsAsync(paymentsToBind.Select(x => x.CreditContractNumber), logger);

			var paymentContractPairs = paymentsToBind
				.Where(x => paymentIds.ContainsKey(x.PaymentGuid) && creditContractIds.ContainsKey(x.CreditContractNumber))
				.ToList();

			foreach (var itm in paymentsToBind.Except(paymentContractPairs))
				fileService.SetBindingResult(itm.PaymentOrder,
					"Загружен в буфер необработанных платежей. Не найден ID по номеру КД",
					string.Empty);

			return paymentContractPairs
				.Select(x => new PaymentOrderLinks
				{
					PaymentId = paymentIds[x.PaymentGuid],
					CreditContractId = creditContractIds[x.CreditContractNumber],
					PaymentOrder = x.PaymentOrder
				})
				.ToList();
		}

		/// <summary>
		/// Получить словарь { GUID платёжки, ID платёжки } по списку GUID-ов платёжек из реестра необработанных платежей (АБС: Бэк-офис/Кредиты/Обработка платежей)
		/// </summary>
		private async Task<Dictionary<long, long>> GetPaymentIdsAsync(IEnumerable<long> paymentGuids, ILogger logger)
		{
			var client = this.GetServiceClient();

			var requests = paymentGuids
				.Distinct()
				.Select(async x =>
				{
					var par = new DsPaymentBufferFindListByParamReq { GUID = x, GUIDSpecified = true };
					var res = await client.dsPaymentBufferFindListByParamAsync(par);
					return (Result: res.DsPaymentBufferFindListByParamRes, GUID: x);
				});

			var responses = await Task.WhenAll(requests);
			await client.CloseAsync();

			var paymentsMap = new Dictionary<long, long>();

			foreach (var response in responses)
				if (response.Result.Status == "OK")
				{
					var paymentByGuid = response.Result
						.PaymentList?
						.FirstOrDefault();

					if (paymentByGuid == null)
						logger.Error("Ошибка запроса списка неразнесённых платежей: отсутствует запись платежа");
					else
						paymentsMap.Add(paymentByGuid.GUID, paymentByGuid.PaymentID);
				}
				else
					logger.Error("Ошибка запроса списка неразнесённых платежей: " + response.Result.ReturnMsg);

			return paymentsMap;
		}

		/// <summary>
		/// Получить словарь { Номер кредитного договора, ID кредитного договора } по номерам кредитных договоров
		/// </summary>
		private async Task<Dictionary<string, long>> GetCreditContractIdsAsync(IEnumerable<string> creditContractNumbers,
			ILogger logger)
		{
			var client = this.GetServiceClient();

			var requests = creditContractNumbers
				.Distinct()
				.Select(async x =>
				{
					var par = new DsLoanBrowseListByParamReq { Number = x };
					var res = await client.dsLoanBrowseListByParamAsync(par);
					return (Result: res.DsLoanBrowseListByParamRes, Number: x);
				});

			var responses = await Task.WhenAll(requests);
			await client.CloseAsync();

			var contractsMap = new Dictionary<string, long>();

			foreach (var response in responses)
			{
				if (response.Result.Status != "OK")
					logger.Error($"Ошибка запроса ID по номеру КД {response.Number}: {response.Result.ReturnMsg}");

				var contractByNumber = response.Result
					.Result?
					.FirstOrDefault();

				if (contractByNumber == null)
					logger.Error($"Ошибка запроса ID по номеру КД {response.Number}: отсутствует результат");
				else
					contractsMap.Add(contractByNumber.Number, contractByNumber.LoanID);
			}

			return contractsMap;
		}

		/// <summary>
		/// Привязать Платёжное поручение к Кредитному договору
		/// </summary>
		private async Task BindPaymentsToCreditContractsAsync(IEnumerable<PaymentOrderLinks> paymentToBind,
			IFileService bindingResultSetter, ILogger logger)
		{
			// AutomaticReversalFlag - флаг автоматического сторнирования и
			// выполнения операций по договору после даты платежа:
			// 1 - не выполнять (по умолчанию)
			// 2 - выполнять

			var requests = paymentToBind
				.Select(x => (
					Request: new DsPaymentBufferExecuteOperationReq
					{
						LoanID = x.CreditContractId,
						PaymentID = x.PaymentId,
						AutomaticReversalFlag = 2,
						AutomaticReversalFlagSpecified = true
					},
					Binding: new PaymentBinding
					{
						LoanId = x.CreditContractId,
						PaymentId = x.PaymentId,
						Date = DateTime.UtcNow,
						Status = PaymentBinding.BindingStatus.Scheduled,
						PaymentOrderId = x.PaymentOrderId
					},
					Order: x.PaymentOrder))
				.ToArray();

			if (!requests.Any())
				logger.Information("Список привязок пуст");

			await this.paymentOrdersService
				.AddPaymentBindings(requests.Select(x => x.Binding));

			foreach (var req in requests)
				bindingResultSetter?.SetBindingResult(req.Order, "Запланирована", string.Empty);

			var tasks = requests
				.Select(async x =>
				{
					var req = x.Request;
					var binding = x.Binding;

					binding.Status = PaymentBinding
						.BindingStatus
						.BeingProcessed;
					bindingResultSetter?.SetBindingResult(x.Order, "Обрабатывается", string.Empty);

					try
					{
						var client = this.GetServiceClient();
						var r = await client
							.dsPaymentBufferExecuteOperationAsync(x.Request);
						await client.CloseAsync();

						var res = r.DsPaymentBufferExecuteOperationRes;

						if (res.ReturnCodeSpecified)
						{
							string errMsg = string.IsNullOrEmpty(res.ReturnMsg) ?
								"не указана" : res.ReturnMsg;
							logger.Error("Привязка ПП {0} КД {1} Ошибка: {2}", req.PaymentID, req.LoanID, errMsg);
							binding.Status = PaymentBinding.BindingStatus.ProccessedWithError;
							binding.BindingErrorMessage = errMsg;
							bindingResultSetter?.SetBindingResult(x.Order, "Завершена", "Ошибка: " + errMsg);
						}
						else
						{
							logger.Information("Привязка ПП {0} КД {1} Успешно", req.PaymentID, req.LoanID);
							binding.Status = PaymentBinding.BindingStatus.ProcessedSuccessfully;
							binding.BindingErrorMessage = "Успешно";
							bindingResultSetter?.SetBindingResult(x.Order, "Завершена", "Успешно");
						}
					}
					catch (Exception e)
					{
						logger.Error("Привязка ПП {0} КД {1} Ошибка: {2}", req.PaymentID, req.LoanID, e.Message);
						binding.Status = PaymentBinding.BindingStatus.ProccessedWithError;
						binding.BindingErrorMessage = e.Message;
						bindingResultSetter?.SetBindingResult(x.Order, "Завершена", "Ошибка: " + e.Message);
					}
				});

			await Task.WhenAll(tasks);

			await this.paymentOrdersService
				.SaveChangesAsync();
		}

		private LOANCREDITWSPORTTYPEClient GetServiceClient()
		{
			var client = new LOANCREDITWSPORTTYPEClient();
			client.Endpoint.Address = new EndpointAddress(this.absOptions.Path);
			client.ClientCredentials.UserName.UserName = this.absOptions.UserName;
			client.ClientCredentials.UserName.Password = this.absOptions.Password;
			client.Endpoint.Binding.SendTimeout =
				TimeSpan.FromSeconds(this.absOptions.SendTimeout);
			return client;
		}
	}
}