using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataUploader.Installers
{
    public class HealthChecksInstaller : IInstaller
    {
        //Microsoft.Extensions.Diagnostics.Healthchecks.EntityFrameworkCore <-nuget
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecks();
        }
    }
}
