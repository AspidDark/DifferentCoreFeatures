using DPSWeb.Data;
using DPSWeb.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DPSWeb.Installers
{
    public class DBInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DataContext>(options =>
               options.UseSqlServer(
                   configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<ISupplementaryAgreementService, SupplementaryAgreementService>();
            services.AddScoped<IInterestChargeService, InterestChargeService>();
          
        }
    }
}
