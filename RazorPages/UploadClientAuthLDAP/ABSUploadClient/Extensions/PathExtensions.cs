using System;
using System.IO;

namespace ABSUploadClient.Extensions
{
	public static class Path
	{
		public static string GetTempPath()
		{
			var path =  System.IO.Path
				.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temp");
			
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);

			return path;
		}
	}
}