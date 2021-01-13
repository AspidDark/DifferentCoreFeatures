using DPSService.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DPSService.Installers
{
    public static class DBInstaller
    {
		public  static void InstallDBServices(this IServiceCollection services, IConfiguration configuration)
		{
			//IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
			//// Duplicate here any configuration sources you use.
			//configurationBuilder.AddJsonFile("AppSettings.json");
			//IConfiguration configuration = configurationBuilder.Build();
			services.AddDbContext<DataContext>(options =>
			{
				options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
			});

			services.BuildServiceProvider()
				.GetService<DataContext>()
				.Database
				.Migrate();
		}
	}
}
