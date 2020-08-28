using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;

namespace ABSUploadClient
{
	public class Program
	{
		private static Logger Logger;

		public static void Main(string[] args)
		{
			try
			{
				var config = new ConfigurationBuilder()
					.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
					.AddJsonFile("appsettings.json", false)
					.Build();

				Logger = new LoggerConfiguration()
					.ReadFrom
					.Configuration(config)
					.CreateLogger();

				AppDomain.CurrentDomain
					.UnhandledException += UnhandledException;
				TaskScheduler
					.UnobservedTaskException += UnobservedException;

				Logger.Information("-Starting host-");

				Host.CreateDefaultBuilder(args)
					.UseSerilog()
					.ConfigureWebHostDefaults(x => x.UseStartup<Startup>())
					.Build()
					.Run();
			}
			catch (Exception e)
			{
				Logger.Fatal(e, "-Host terminated unexpectedly-");
			}
		}

		private static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Logger.Error("Unhandled exception {0}", e.ExceptionObject);
		}

		private static void UnobservedException(object sender, UnobservedTaskExceptionEventArgs e)
		{
			Logger.Error("Unobserved exception {0}", e.Exception?.Flatten());
		}
	}
}