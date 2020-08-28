using ABSUploadClient.Models.Convertion;
using ABSUploadClient.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UploadClient.Models.Convertion;

namespace UploadClient.Installers
{
	public class MainInstallers : IInstaller
	{
		public void InstallServices(IServiceCollection services, IConfiguration configuration)
		{
			services.AddTransient<ConvertToExcel>();
			services.AddTransient<ExcelService>();
			services.AddTransient<DbfService>();
			services.AddTransient<FileBackupService>();
			services.AddTransient<PaymentSendService>();
			services.AddTransient<PaymentOrdersService>();

			services.AddTransient<IModuleBreafService, ModuleBreafService>();
			services.AddTransient<IPaymentBindingService, PaymentBindingService>();
		}
	}
}