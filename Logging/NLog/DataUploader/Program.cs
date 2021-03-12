using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using NLog.Web;
using NLog;
using System.Threading.Tasks;

namespace DataUploader
{
    public class Program
    {
        private static Logger Log;
        public static void Main(string[] args)
        {
            try
            {
                Log = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
                CreateHostBuilder(args).Build().Run();


            }
            catch (Exception exception)
            {
                //NLog: catch setup errors
                Log.Error(exception, "Stopped program because of exception" + exception.Message);
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>().UseIISIntegration();
                })
            .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                })
                .UseNLog();
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Fatal("Необработанное исключение {0}", e.ExceptionObject.ToString(), string.Empty);
            Console.WriteLine("Необработанное исключение " + e.ExceptionObject.ToString());
        }

        private static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Log.Fatal("Необработанное исключение в потоке из пула {0}", e.Exception.GetBaseException().ToString(), string.Empty);
            Console.WriteLine("Необработанное исключение в потоке из пула " + e.Exception.GetBaseException().ToString());
        }
    }
}
