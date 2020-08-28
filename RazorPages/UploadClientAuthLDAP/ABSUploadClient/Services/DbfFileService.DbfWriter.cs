using System.Text;
using DotNetDBF;

namespace ABSUploadClient.Services
{
	partial class DbfFileService
	{
		private class DbfWriter : DBFWriter
		{
			public DbfWriter(DBFField[] fields, Encoding encoding, byte languageCode)
			{
				this.Fields = fields;
				this.CharEncoding = encoding;
				this.LanguageDriver = languageCode;
			}
		}
	}
}