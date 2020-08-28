using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ABSUploadClient.Entity;
using ABSUploadClient.Entity.EntityModel;
using ABSUploadClient.Options;
using ABSUploadClient.Services;
using ABSUploadClient.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;

namespace ABSUploadClient.UD
{
	public class IndexModel : PageModel
	{
		private readonly PaymentOrdersContext context;
		private readonly FileBackupService backupService;
		private readonly VisualSettings visualSettings;

		public UploadedDocumentsPagination UploadedDocumentsPagination { get; set; }

		public IndexModel(PaymentOrdersContext context, FileBackupService backupService,
			VisualSettings visualSettings)
		{
			this.context = context;
			this.backupService = backupService;
			this.visualSettings = visualSettings;
		}

		public IList<UploadedDocument> UploadedDocument { get; set; }

		public async Task OnGetAsync(int id = 1)
		{
			var count = await this.context
				.UploadedDocuments
				.CountAsync();

			this.UploadedDocument = await this.context
				.UploadedDocuments
				.OrderByDescending(x => x.UploadTime)
				.Skip((id - 1) * this.visualSettings.UploadsOnPageCount)
				.Take(this.visualSettings.UploadsOnPageCount)
				.ToListAsync();

			this.UploadedDocumentsPagination =
				new UploadedDocumentsPagination(count, id, this.visualSettings.UploadsOnPageCount);
		}

		public async Task<IActionResult> OnPostGetId(int id)
		{
			this.UploadedDocument = await this.context
				.UploadedDocuments
				.OrderByDescending(x => x.UploadTime)
				.ToListAsync();

			var path = await this.backupService
				.GetDocumentLocation(id);

			if (!System.IO.File.Exists(path))
				return this.BadRequest("Ошибка: файл не найден");

			var stream =
				new FileStream(path, FileMode.Open);

			var header = new MediaTypeHeaderValue("application/octet-stream");
			return new FileStreamResult(stream, header) { FileDownloadName = "Report" + Path.GetFileName(path) };
		}
	}
}