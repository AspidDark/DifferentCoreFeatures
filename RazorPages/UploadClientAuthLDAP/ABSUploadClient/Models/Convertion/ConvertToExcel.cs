using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ABSUploadClient.Services;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UploadClient.Models.Convertion
{
	public sealed class ConvertToExcel
	{
		private readonly FileBackupService backupService;
		private readonly ILogger<ConvertToExcel> _logger;

		public ConvertToExcel(FileBackupService backupService, ILogger<ConvertToExcel> logger)
		{
			this.backupService = backupService;
			_logger = logger;
		}

		public string Convert(IFormFile formFile)
		{
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
			var inputFile = ReadString(formFile);

			// 2. Распарсили 1С-файл, получили список платёжек. Отфильтровали список, отсортировали: платёжки, требующие привязки, - в начале списка
			var paymentsFrom1C = LoadFrom1C(inputFile)
													 //.Where(x => x["ДатаПоступило"] != null)           // Исходящие платежи
													 //.Where(x => x["ПлательщикИНН"]?.ToString() != x["ПолучательИНН"]?.ToString()) // Внутренние переводы ООО МФК "Быстроденьги"
													 .OrderBy(x => x["Плательщик1"] != null && x["Плательщик1"].ToString().StartsWith("УФК") ? 1 : 2) // Взыскания по просуженным займам через Пенсионный фонд - в первую очередь
													 .ThenBy(x => x["Плательщик1"] != null && x["Плательщик1"].ToString().StartsWith("ПАО") ? 1 : 2) // Самостоятельная оплата от заёмщика - во вторую очередь 
													 .ThenBy(x => x["НазначениеПлатежа"] != null && x["НазначениеПлатежа"].ToString().ToLower().Contains("инкас") ? 2 : 1); // Платежи от инкассации - в последнюю очередь 


			_logger.LogInformation($"Распарсили 1С-файл, получили список платёжек. Отфильтровали список, отсортировали: платёжки, требующие привязки, - в начале списка | Имя файла {formFile.FileName}");

			// 3. Экспортировали список в Excel для последующей ручной обработки - оператор должен заполнить поле НомерКредитногоДоговора
			var workbook = GetExcelWorkbook(paymentsFrom1C, "Документы");
			_logger.LogInformation($"Экспортировали список в Excel для последующей ручной обработки - оператор должен заполнить поле НомерКредитногоДоговора | Имя файла {formFile.FileName}");

			return this.SaveWorkbookToMemoryStream(workbook, formFile.FileName);
		}

		/// <summary> Загрузка платежей через разбор 1с-формата выгрузки платёжек </summary>
		/// <param name="input1CDocument">выгрузка платёжек в формате 1CClientBankExchange версии 1.02 и выше</param>
		private static IEnumerable<JObject> LoadFrom1C(string input1CDocument)
		{
			var regex = new Regex(@"СекцияДокумент(?:(?:\s|\t)*=(?:\s|\t)*(?<doctype>(?:\w|\s)*)){0,1}\n(?:(?<kv>(.)*?)\n)*?КонецДокумента", RegexOptions.Singleline);
			var matches = regex.Matches(input1CDocument).OfType<Match>().ToList();

			foreach (var match in matches)
			{
				var payment = new JObject { { "@Тип", match.Groups["doctype"].Value.Trim() } };

				foreach (var s in match.Groups["kv"].Captures.OfType<Capture>().Select(x => x.Value))
				{
					var kvPair = s.Split('=');
					payment.Add(kvPair[0].Trim(), kvPair.Length == 2 ? kvPair[1].Trim() : null);
				}

				yield return payment;
			}
		}

		private static IXLWorkbook GetExcelWorkbook(IEnumerable<JObject> incomePayments, string worksheetName)
		{
			if (incomePayments == null) throw new ArgumentNullException(nameof(incomePayments));
			if (!incomePayments.Any()) throw new ArgumentOutOfRangeException(nameof(incomePayments));

			var jArray = new JArray(incomePayments);
			var tempTable = JsonConvert.DeserializeObject<DataTable>(jArray.ToString());

			var dataTable = new DataTable(worksheetName);
			dataTable.Columns.Add("НомерКредитногоДоговора");
			var dataColumns = new List<DataColumn>();
			foreach (DataColumn column in tempTable.Columns) dataColumns.Add(column);
			dataTable.Columns.AddRange(dataColumns
																 .Select(x => new DataColumn(x.ColumnName, x.DataType))
																 .OrderBy(x => x.ColumnName == "Номер" ? 1 : 2)
																 .ThenBy(x => x.ColumnName == "ДатаПоступило" ? 1 : 2)
																 .ThenBy(x => x.ColumnName == "Сумма" ? 1 : 2)
																 .ThenBy(x => x.ColumnName == "НазначениеПлатежа" ? 1 : 2)
																 .ThenBy(x => x.ColumnName == "Плательщик1" ? 1 : 2)
																 .ThenBy(x => x.ColumnName == "@Тип" ? 2 : 1)
																 .ToArray());

			foreach (DataRow row in tempTable.Rows)
			{
				var newRow = dataTable.NewRow();
				foreach (DataColumn column in tempTable.Columns)
				{
					newRow[column.ColumnName] = row[column.ColumnName];
				}

				var matchCollection = Regex.Matches(row["НазначениеПлатежа"].ToString(), @"\D+(?<ndoc>9\d{7})(\D|$)");
				if (matchCollection.Count == 1)
				{
					newRow["НомерКредитногоДоговора"] = matchCollection[0].Groups["ndoc"];
				}

				dataTable.Rows.Add(newRow);
			}

			var workbook = new XLWorkbook();
			var worksheet = workbook.Worksheets.Add(dataTable);
			worksheet.Columns().AdjustToContents();
			return workbook;
		}

		private string ReadString(IFormFile file)
		{
			using var reader = new StreamReader(file.OpenReadStream(), Encoding.GetEncoding(1251));
			string result = reader.ReadToEnd();
			return result;
		}

		public string SaveWorkbookToMemoryStream(IXLWorkbook workbook, string inputFileName)
		{
			// 3. Экспортировали список в Excel для последующей ручной обработки - оператор должен заполнить поле НомерКредитногоДоговора
			var excelFileName = Path.ChangeExtension(inputFileName, ".xlsx");
			_logger.LogInformation($"Экспортировали список в Excel для последующей ручной обработки | Имя файла {inputFileName}");

			var filePath = Path
				.Combine(this.backupService.UploadDirectory, excelFileName);
			workbook.SaveAs(filePath);

			return filePath;
		}
	}
}
