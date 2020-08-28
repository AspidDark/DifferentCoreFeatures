using ABSUploadClient.Options;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace UploadClient.Installers
{
    public class SettingsInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            var absConfig = new AbsOptions();
            configuration.Bind("AbsOptions", absConfig);

            services.AddSingleton(absConfig);

            var visualSettings = new VisualSettings();
            configuration.Bind("VisualSettings", visualSettings);

            services.Configure<LdapConfig>(configuration.GetSection("Ldap"));

            services.AddSingleton(visualSettings);
        }
    }
}
