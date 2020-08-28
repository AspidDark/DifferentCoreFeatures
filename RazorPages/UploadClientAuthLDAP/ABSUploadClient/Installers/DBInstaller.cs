using ABSUploadClient.Entity;
using ABSUploadClient.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UploadClient.Installers;

namespace ABSUploadClient.Installers
{
	public class DBInstaller : IInstaller
	{
		public void InstallServices(IServiceCollection services, IConfiguration configuration)
		{
			services.AddDbContext<PaymentOrdersContext>(options =>
			{
				options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
			});

			services.BuildServiceProvider()
				.GetService<PaymentOrdersContext>()
				.Database
				.Migrate();

			//services.AddDefaultIdentity<IdentityUser>()  //nuget=> Microsoft.AspNetCoreIdentity.UI
			//    .AddRoles<IdentityRole>()                                    //  .AddDefaultUI(UIFramework.Bootstrap4)
			//    .AddEntityFrameworkStores<DataContext>();

			//services.AddScoped<IPostService, PostService>();
		}
	}
}
