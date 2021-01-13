using DPSWeb.Reports;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DPSWeb.Installers
{
    public class ReportInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IInterestChargeReport, InterestChargeReport>();
            services.AddScoped<ISupplementaryAgreementReport, SupplementaryAgreementReport>();
        }
    }
}
