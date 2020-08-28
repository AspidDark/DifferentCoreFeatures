using System;

namespace ABSUploadClient.Extensions
{
	public static class GuidExtensions
	{
		/// <summary>
		/// Конвертирует Guid в тип long
		/// </summary>
		public static long ToLong(this Guid guid)
		{
			return BitConverter.ToInt64(guid.ToByteArray(), 0);
		}
	}
}