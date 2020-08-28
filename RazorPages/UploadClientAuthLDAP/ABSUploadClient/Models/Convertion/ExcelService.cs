using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ABSUploadClient.Services;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;

namespace UploadClient.Models.Convertion
{
	public class ExcelService
	{
		private readonly PaymentSendService paymentSendService;
		private readonly FileBackupService backupService;

		public ExcelService(PaymentSendService paymentSendService, FileBackupService backupService)
		{
			this.paymentSendService = paymentSendService;
			this.backupService = backupService;
		}

		public async Task<IEnumerable<string>> Parse(IFormFile file, string moduleBrief, string authData)
		{
			using var logger = LogService
				.CreateLogger(file.FileName, out string logPath);

			logger.Information("--- Начало обработки файла " + file.FileName + " ---");

			try
			{
				string resPath = await this.backupService
					.Save(file, authData, moduleBrief);

				var workbook = new XLWorkbook(resPath);
				var excelService = new ExcelFileService(workbook, "Документы", "Статус задачи", "Результат привязки");

				await this.paymentSendService
					.SendPaymentOrders(excelService, moduleBrief, authData, logger);

				logger.Information("--- Окончание обработки файла " + file.FileName + " ---");
				return new string[] { resPath, logPath };
			}
			catch (Exception e)
			{
				logger.Fatal(e, "Ошибка обработки файла " + file.FileName);
				throw;
			}
		}
	}
}