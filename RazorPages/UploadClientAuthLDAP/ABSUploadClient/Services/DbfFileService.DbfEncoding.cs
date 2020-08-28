using System;
using System.ComponentModel;
using System.Linq;

namespace ABSUploadClient.Services
{
	partial class DbfFileService
	{
		public enum DbfEncoding
		{
			[Description("ibm437")]
			IBM437 = 0x01,

			[Description("ibm850")]
			IBM850 = 0x02,

			[Description("windows-1252")]
			Windows1252 = 0x03,

			[Description("cp866")]
			CP866 = 0x65,

			[Description("cp866")]
			CP866_ = 0x26,

			[Description("windows-1251")]
			Windows1251 = 0xC9,
		}
	}

	public static class EnumExtensions
	{
		public static string GetDescription(this Enum item)
		{
			var typeDesc = typeof(DescriptionAttribute);
			var typeEnum = item.GetType();

			return typeEnum
				.GetMember(Enum.GetName(typeEnum, item))
				.FirstOrDefault()?
				.GetCustomAttributes(typeDesc, false)
				.Cast<DescriptionAttribute>()
				.FirstOrDefault()?
				.Description;
		}
	}
}