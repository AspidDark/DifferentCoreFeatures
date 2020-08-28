using System;
using System.IO;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace ABSUploadClient.Services
{
	public class LogService
	{
		public static Logger CreateLogger(string name, out string path)
		{
			path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
				"Logs", $"{name}-{DateTime.Now:yyyy.MM.dd}-upload.log");

			if (File.Exists(path)) File.Delete(path);
			string template =
				"{Timestamp:yyyy-MM-dd HH:mm:ss.fff}|{Level:u3}| {Message:lj}{NewLine}{Exception}";

			return new LoggerConfiguration()
				.WriteTo
				.File(path, LogEventLevel.Information, template)
				.CreateLogger();
		}
	}
}