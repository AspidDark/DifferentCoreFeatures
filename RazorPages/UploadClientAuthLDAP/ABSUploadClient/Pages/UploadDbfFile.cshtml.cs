using System;
using System.IO;
using System.Threading.Tasks;
using ABSUploadClient.Models.Convertion;
using ABSUploadClient.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace ABSUploadClient.Pages
{
	public class UploadDbfFileModel : PageModel
	{
		private readonly DbfService dbfService;
		private readonly ILogger<UploadDbfFileModel> logger;

		public UploadDbfFileModel(DbfService dbfService, ILogger<UploadDbfFileModel> logger)
		{
			this.dbfService = dbfService;
			this.logger = logger;
		}

		[BindProperty]
		public IFormFile FileUpload { get; set; }

		public void OnGet() { }

		public async Task<IActionResult> OnPost()
		{
			if (string.IsNullOrEmpty(this.FileUpload?.FileName))
				return this.BadRequest("Критическая ошибка, не указано имя файла");

			try
			{
				var paths = await this.dbfService
					.Parse(this.FileUpload, "PochtaR", this.HttpContext.User.Identity.Name);
				string archive = Path
					.Combine(Extensions.Path.GetTempPath(), "absupload.zip");

				new ZipService()
					.Compress(paths, archive);

				var stream =
					new FileStream(archive, FileMode.Open);

				var header = new MediaTypeHeaderValue("application/zip");
				return new FileStreamResult(stream, header) { FileDownloadName = "archive.zip" };
			}
			catch (Exception e)
			{
				this.logger.LogError(e, $"Ошибка обработки {this.FileUpload.FileName}");
				return this.BadRequest($"Ошибка обработки '{this.FileUpload.FileName}': {e.Message}");
			}
		}
	}
}