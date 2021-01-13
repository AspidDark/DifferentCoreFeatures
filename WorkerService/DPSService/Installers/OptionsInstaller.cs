using DPSService.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DPSService.Installers
{
    public static class OptionsInstaller
    {
		public static void InstallOptions(this IServiceCollection services, IConfiguration configuration)
		{
			var calculatorsSavePathOptions = new CalculatorsSavePathOptions();
			configuration.Bind("CalculatorsSavePath", calculatorsSavePathOptions);
			services.AddSingleton(calculatorsSavePathOptions);

			var excelTemplaesOptions = new ExcelTemplaesOptions();
			configuration.Bind("ExcelTemplaesOptions", excelTemplaesOptions);
			services.AddSingleton(excelTemplaesOptions);
		}
	}
}
