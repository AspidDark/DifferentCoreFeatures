using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace DataUploader.Installers
{
    public static class InstallerExtensions
    {
        public static void InstallServicesInAssemblies(this IServiceCollection services, IConfiguration configuration)
        {
            //Geting instances of all install classes
            var installers = typeof(Startup).
                Assembly.ExportedTypes.Where(x => typeof(IInstaller).IsAssignableFrom(x)
            && !x.IsInterface
            && !x.IsAbstract).Select(Activator.CreateInstance)
            .Cast<IInstaller>()
            .ToList();

            //instaling them
            installers.ForEach(installer => installer.InstallServices(services, configuration));
        }
    }
}
