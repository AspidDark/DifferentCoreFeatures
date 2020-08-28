using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace ABSUploadClient.Services
{
	public class ZipService
	{
		public void Compress(IEnumerable<string> inputPaths, string outputPath)
		{
			if (File.Exists(outputPath)) File.Delete(outputPath);
			var zip = ZipFile.Open(outputPath, ZipArchiveMode.Create);

			foreach (var path in inputPaths)
				zip.CreateEntryFromFile(path, Path.GetFileName(path));
			zip.Dispose();
		}
	}
}