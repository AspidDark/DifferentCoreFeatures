using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

namespace DPSWeb.Installers
{
    public class SwaggerInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("DpsWeb", new OpenApiInfo { Title = "DpsWeb API", Version = "v1" });

                //x.ExampleFilters();
            });

            services.AddSwaggerExamplesFromAssemblyOf<Startup>();
        }
    }
}
