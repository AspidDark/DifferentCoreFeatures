using System;
using System.Collections.Generic;
using DPSService.Installers;
using System.Threading.Tasks;
using DPSService.Jobs;
using DPSService.Quartz;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Serilog;
using Serilog.AspNetCore;
using Serilog.Events;
using Microsoft.Extensions.Configuration;
using DPSService.DB.Repository;
using DPSService.Services;

namespace DPSService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            // Duplicate here any configuration sources you use.
            configurationBuilder.AddJsonFile("AppSettings.json");
            IConfiguration configuration = configurationBuilder.Build();

            Log.Logger = new LoggerConfiguration()
                 .MinimumLevel.Debug()
                 .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                 .Enrich.FromLogContext()
                 .WriteTo.File(configuration.GetValue<string>("LoggerPath"))
                 .CreateLogger();

            try
            {
                Log.Information("Starting up the service");
                CreateHostBuilder(args).Build().Run();
                return;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "There was a problem starting the serivce");
                return;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            // Duplicate here any configuration sources you use.
            configurationBuilder.AddJsonFile("AppSettings.json");
            IConfiguration configuration = configurationBuilder.Build();

            return Host.CreateDefaultBuilder(args)
                 .UseWindowsService()
                 .ConfigureServices((hostContext, services) =>
                 {
                     services.BuildServiceProvider();
                     services.AddSingleton<ILoggerFactory, Serilog.Extensions.Logging.SerilogLoggerFactory>();
                   //  services.AddSingleton<IApplicationScheduler, ApplicationScheduler>();
                     services.AddTransient(typeof(CreateReportJob));
                     services.AddHostedService<Worker>();
                    // services.AddSingleton(typeof(IGenericRepository<>), typeof(EFGenericRepository<>));
                    // //services.UseQuartz(new JobFactory(services.BuildServiceProvider())).Wait();
                     services.InstallOptions(configuration);
                     services.InstallDBServices(configuration);
                 })
                 .UseSerilog();
        }
    }
}
