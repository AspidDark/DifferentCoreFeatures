using DataUploader.CascadAuthirization;
using DataUploader.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataUploader.Installers
{
    public class HttpInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            var httpOptions = new HttpOptions();
            configuration.Bind(nameof(HttpOptions), httpOptions);
            services.AddSingleton(httpOptions);

            services.AddScoped<ICascadClientFactory, CascadClientFactory>();
        }
    }
}
