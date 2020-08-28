using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ABSUploadClient.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using UploadClient.Models.Convertion;

namespace ABSUploadClient.Pages
{
	public class UploadExcelFilePageModel : PageModel
	{
		private readonly ExcelService _excelParseAndSend;
		private readonly IModuleBreafService _moduleBreafService;
		private readonly ILogger<UploadExcelFilePageModel> _logger;

		public UploadExcelFilePageModel(ExcelService excelParseAndSend, IModuleBreafService moduleBreafService,
			ILogger<UploadExcelFilePageModel> logger)
		{
			_excelParseAndSend = excelParseAndSend;
			_moduleBreafService = moduleBreafService;
			_logger = logger;
		}

		public SelectList Values { get; set; }

		[BindProperty]
		public IFormFile FileUpload { get; set; }

		[BindProperty]
		public string SelectedValue { get; set; }

		public void OnGet()
		{
			var valuesTemp = _moduleBreafService.GetAllModuleBreaf().Select(m => m.ModuleValue).ToList();
			Values = new SelectList(valuesTemp);
		}

		public async Task<IActionResult> OnPost()
		{
			var authData = HttpContext.User.Identity.Name;
			_logger.LogInformation($"OnPost | FileName={FileUpload?.FileName} | Провайдер {SelectedValue}");

			if (FileUpload == null)
				return BadRequest("Критическая ошибка, не указано имя файла");
			else
				try
				{
					var paths = await _excelParseAndSend
						.Parse(FileUpload, SelectedValue, authData);
					string archive = Path
						.Combine(Extensions.Path.GetTempPath(), "absupload.zip");

					new ZipService()
						.Compress(paths, archive);

					var stream =
						new FileStream(archive, FileMode.Open);

					return new FileStreamResult(stream, new MediaTypeHeaderValue("application/zip"))
					{
						FileDownloadName = "archive.zip"
					};
				}
				catch (Exception e)
				{
					_logger.LogError(e, $"Ошибка обработки {FileUpload.FileName}");
					return BadRequest($"Ошибка обработки {FileUpload.FileName} " +
						e.Message + e.StackTrace);
				}
		}
	}
}