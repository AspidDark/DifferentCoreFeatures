using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using UploadClient.Models.Convertion;

namespace ABSUploadClient.Pages
{
	public class Upload1CFileModel : PageModel
	{
		private readonly ConvertToExcel excelConverter;
		private readonly ILogger<Upload1CFileModel> logger;

		public Upload1CFileModel(ConvertToExcel excelConverter, ILogger<Upload1CFileModel> logger)
		{
			this.excelConverter = excelConverter;
			this.logger = logger;
		}

		[BindProperty]
		public IFormFile FileUpload { get; set; }

		public void OnGet() { }

		public IActionResult OnPost()
		{
			if (string.IsNullOrEmpty(this.FileUpload?.FileName))
				return this.BadRequest("����������� ������, �� ������� ��� �����");

			try
			{
				var path = this.excelConverter
					.Convert(this.FileUpload);

				var stream =
					new FileStream(path, FileMode.Open);

				var header = new MediaTypeHeaderValue("application/octet-stream");
				return new FileStreamResult(stream, header) { FileDownloadName = Path.GetFileName(path) };
			}
			catch (Exception e)
			{
				this.logger.LogError(e, $"������ ��������� {this.FileUpload.FileName}");
				return this.BadRequest($"������ ��������� '{this.FileUpload.FileName}': {e.Message}");
			}
		}
	}
}
