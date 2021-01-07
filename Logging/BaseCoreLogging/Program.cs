using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BaseCoreLogging
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            var logger = host.Services.GetRequiredService<ILogger<Program>>(); //Getting logger service
            logger.LogInformation("Application is started");
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging((context, logging)=> 
                {
                    logging.ClearProviders(); //clearing everyone who listen to log events
                    logging.AddConfiguration(context.Configuration.GetSection("Logging")); //takes from appsettings.json or Enviroment Variables values
                    logging.AddDebug(); 
                    logging.AddConsole();
                    // EventSource, EventLog, TraceSource, AzureAppservicesFile, AzureAppservicesBlob, ApplicationInsights
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
