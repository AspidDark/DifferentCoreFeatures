using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;

namespace LogingWithSerilog
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            try
            {
                Log.Information("Application Straring Up");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application Faild to start");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }


        //Serilog 
        // rollingInterval : RollingInterval.Day Как часто
        // hostingContext.Configuration.GetSection("LogPath") путь из appsettings.json
        // 
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args).
            UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration
            .ReadFrom.Configuration(hostingContext.Configuration)
            .Enrich.FromLogContext().WriteTo.File(hostingContext.Configuration.GetSection("Serilog:LogPath").Value, 
                rollingInterval : RollingInterval.Day))
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        /////OR!
        /*
         public static void Main(string[] args)
        {

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                 .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration).WriteTo.File(configuration.GetSection("LogPath").Value)
                .CreateLogger();

            try
            {
                Log.Information("Application Straring Up");
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application Faild to start");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args).
            UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration
            .ReadFrom.Configuration(hostingContext.Configuration)
            .Enrich.FromLogContext().WriteTo.File(hostingContext.Configuration.GetSection("LogPath").Value, 
                rollingInterval : RollingInterval.Day))
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

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
         * */
    }
}
