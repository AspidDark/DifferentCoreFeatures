using ApiVersioning.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.Versioning.Conventions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace ApiVersioning
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            ///Versioning
            services.AddApiVersioning(options=> 
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                //options.DefaultApiVersion = ApiVersion.Default;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ApiVersionReader = new MediaTypeApiVersionReader("version"); //from where to read version... header or version header... it overrides attributes in controller
                // options.ApiVersionReader = new HeaderApiVersionReader("X-Version"); //If we want to create own header

                //options.ApiVersionReader = ApiVersionReader.Combine(  //Combine multyple rules Rules apply one OR!! Another
                //    new MediaTypeApiVersionReader("version"),
                //    new HeaderApiVersionReader("X-Version")
                //    );

                options.ReportApiVersions = true; //Show api versions in response header

                options.Conventions.Controller<VersionongController>()
                .HasDeprecatedApiVersion(1, 0) //same as attribute usage
                .HasApiVersion(2, 0) //same as attribute usage
                .Action(typeof(VersionongController).GetMethod(nameof(VersionongController.GetProductV2))!) //same as attribute usage
                .MapToApiVersion(2,0); //same as attribute usage

            }); 


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ApiVersioning", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiVersioning v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
