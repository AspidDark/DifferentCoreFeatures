using System;
using System.IO;
using System.Threading.Tasks;
using ABSUploadClient.Entity;
using ABSUploadClient.Entity.EntityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ABSUploadClient.Services
{
	public class FileBackupService
	{
		private readonly PaymentOrdersContext dbContext;

		public FileBackupService(PaymentOrdersContext dbContext)
		{
			this.dbContext = dbContext;
		}

		public async Task<string> Save(IFormFile formFile, string userName, string source)
		{
			string fileName = formFile.FileName;
			string backupFileName = $"{DateTime.Now:yyyy_MM_dd_hh_mm_ss}_{formFile.FileName}";

			if (!Directory.Exists(this.UploadDirectory))
				Directory.CreateDirectory(this.UploadDirectory);

			var path = Path.Combine(this.UploadDirectory, backupFileName);

			using var stream = new FileStream(path, FileMode.Create);
			await formFile.CopyToAsync(stream);

			var uploadedDocument = new UploadedDocument
			{
				UploadTime = DateTime.Now,
				UserName = userName,
				SourceValue = source,
				FileName = fileName,
				BackupFileName = backupFileName
			};

			await this.dbContext
				.UploadedDocuments
				.AddAsync(uploadedDocument);

			await this.dbContext.SaveChangesAsync();
			return path;
		}

		public async Task<string> GetDocumentLocation(int id)
		{
			var item = await this.dbContext.UploadedDocuments
				.AsNoTracking()
				.FirstOrDefaultAsync(x => x.Id == id);
			return Path.Combine(this.UploadDirectory, item.BackupFileName);
		}

		public string UploadDirectory
		{
			get
			{
				return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Uploads");
			}
		}
	}
}