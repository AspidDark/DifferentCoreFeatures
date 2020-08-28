using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ABSUploadClient.Dto;
using ABSUploadClient.Entity;
using ABSUploadClient.Entity.EntityModel;
using Microsoft.EntityFrameworkCore;
using UploadClient.Models.AbsDTO;

namespace ABSUploadClient.Services
{
	public class PaymentOrdersService
	{
		private readonly PaymentOrdersContext dbContext;

		public PaymentOrdersService(PaymentOrdersContext dbContext)
		{
			this.dbContext = dbContext;
		}

		public IEnumerable<PaymentOrder> FilterExistingOrders(IEnumerable<PaymentOrder> paymentOrders)
		{
			var existingPaymentOrders = this.dbContext
				.PaymentOrders
				.AsNoTracking();

			// Дата, Номер, Сумма, Плательщик, Получатель, Назначение
			return paymentOrders
				.Where(x => !existingPaymentOrders.Any(y =>
					y.CreditContractNumber == x.CreditContractNumber &&
					y.Number == x.Number &&
					y.Amount == x.Amount &&
					y.PayerName == x.PayerName &&
					y.RequestRecivedOn == DateTime.Today));
		}

		public async Task<List<PaymentBindingMap>> SaveAbsSentResult(IReadOnlyList<PaymentOrder> paymentOrders,
			IEnumerable<AbsSentResultDto> absSendError, string userAuthData)
		{
			var paymentBindingMaps = new List<PaymentBindingMap>();
			var errors = absSendError?
				.ToDictionary(x => x.LinkId, x => x.ErrorMessage);

			var entities = paymentOrders.Select((x, i) =>
			{
				var item = new PaymentOrderEntity
				{
					Id = Guid.NewGuid(),
					RequestRecivedOn = DateTime.Today,
					CreditContractNumber = x.CreditContractNumber,
					Number = x.Number,
					IncomeDate = x.IncomeDate,
					Amount = x.Amount,
					Description = x.Description,
					PayerName = x.PayerName,
					AuthentificationData = userAuthData
				};

				var paymentBindingMap = new PaymentBindingMap
				{
					PaymentOrderId = item.Id,
					PaymentOrder = x
				};

				paymentBindingMaps.Add(paymentBindingMap);

				if (errors != null && errors.ContainsKey(i))
					item.Comment = errors[i];
				return item;
			});

			await this.dbContext.PaymentOrders
				.AddRangeAsync(entities);
			await this.dbContext.SaveChangesAsync();

			return paymentBindingMaps;
		}

		public async Task AddPaymentBindings(IEnumerable<PaymentBinding> items)
		{
			await this.dbContext
				.PaymentBindings
				.AddRangeAsync(items);
			await this.dbContext.SaveChangesAsync();
		}

		public Task SaveChangesAsync()
		{
			return this.dbContext.SaveChangesAsync();
		}
	}
}