using LogService.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace LogService
{
    public class Program
    {
        private static ILogger Log;

        public static void Main(string[] args)
        {
            try
            {
                var host = CreateHostBuilder(args).Build();
                Log = host.Services.GetRequiredService<ILogger<Program>>();
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
                Log.LogInformation("Host created.");
                host.Run();
            }
            catch (Exception ex)
            {
                Exception innerException = ex.GetOriginalException();
                Console.WriteLine("Stopped program because of exception" + innerException.Message);
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging((context, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddConfiguration(context.Configuration.GetSection("Logging"));
                    logging.AddDebug();
                    logging.AddConsole();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.LogCritical("Необработанное исключение {0}", e.ExceptionObject.ToString(), string.Empty);
            Console.WriteLine("Необработанное исключение " + e.ExceptionObject.ToString());
        }

        private static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Log.LogCritical("Необработанное исключение в потоке из пула {0}", e.Exception.GetBaseException().ToString(), string.Empty);
            Console.WriteLine("Необработанное исключение в потоке из пула " + e.Exception.GetBaseException().ToString());
        }
    }
}
