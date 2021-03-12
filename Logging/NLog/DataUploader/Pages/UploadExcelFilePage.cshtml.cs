using System;
using System.Threading.Tasks;
using DataUploader.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ABSUploadClient.Pages
{
	public class UploadExcelFilePageModel : PageModel
	{
		private readonly ILogger<UploadExcelFilePageModel> _logger;
		private readonly IExcelParser _excelParser;

        public UploadExcelFilePageModel(ILogger<UploadExcelFilePageModel> logger, IExcelParser excelParser)
        {
            _logger = logger;
            _excelParser = excelParser;
        }

        [BindProperty]
		public IFormFile FileUpload { get; set; }

		[BindProperty]
		public int SubDivisionId { get; set; }

        public void OnGet()
		{
		}

		public async Task<IActionResult> OnPost()
		{
			_logger.LogInformation($"OnPost | FileName={FileUpload?.FileName}");

			if (FileUpload == null)
				return BadRequest("Критическая ошибка, не указано имя файла");
			else
				try
				{
					await _excelParser.Parse(FileUpload, SubDivisionId);
					return new OkResult();
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