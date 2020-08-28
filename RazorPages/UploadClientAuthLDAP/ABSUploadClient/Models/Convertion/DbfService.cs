using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ABSUploadClient.Services;
using Microsoft.AspNetCore.Http;

namespace ABSUploadClient.Models.Convertion
{
	public partial class DbfService
	{
		private readonly PaymentSendService paymentSendService;
		private readonly FileBackupService backupService;

		public DbfService(PaymentSendService paymentSendService, FileBackupService backupService)
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

				var dbfService = new DbfFileService(resPath, "bindstatus", "bindresult");

				await this.paymentSendService
					.SendPaymentOrders(dbfService, moduleBrief, authData, logger);

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