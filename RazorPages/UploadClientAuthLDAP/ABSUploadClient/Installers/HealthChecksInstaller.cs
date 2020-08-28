using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ABSUploadClient.Entity;
using ABSUploadClient.HealthCheks;

namespace UploadClient.Installers
{
    public class HealthChecksInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecks()
                .AddDbContextCheck<PaymentOrdersContext>()
                .AddCheck<HealthAbsIntegration>("ABS Availability check");
        }
    }
}
