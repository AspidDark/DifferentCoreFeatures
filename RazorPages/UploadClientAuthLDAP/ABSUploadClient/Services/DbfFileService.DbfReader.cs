using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using DotNetDBF;

namespace ABSUploadClient.Services
{
	partial class DbfFileService
	{
		private class DbfReader : DBFReader
		{
			private readonly Dictionary<string, int> indexes;
			private readonly string[] columns = new string[] {
				"ELCHECK",
				"ACCOUNT",
				"BIC",
				"CORACCOUNT",
				"ACCEPTDATE",
				"MSG",
				"PAYSUM",
				"FROMNAME",
				"ACCEPTNUM"
			};

			public DbfReader(Stream stream) : base(stream)
			{
				var header = typeof(DBFReader)
					.GetField("_header", BindingFlags.NonPublic | BindingFlags.Instance)
					.GetValue(this) as DBFHeader;

				this.LanguageCode = (byte)typeof(DBFHeader)
					.GetProperty("LanguageDriver", BindingFlags.NonPublic | BindingFlags.Instance)
					.GetValue(header);

				int code = this.LanguageCode;
				if (!typeof(DbfEncoding).IsEnumDefined(code))
					code = this.LanguageCode = (byte)DbfEncoding.CP866;
					//throw new InvalidOperationException($"неизвестная кодировка {code}");

				string enc = ((DbfEncoding)code)
					.GetDescription();

				this.CharEncoding = Encoding.GetEncoding(enc);

				this.indexes = this.columns
					.Select(x => new
					{
						Name = x,
						Index = this.ColumnIndex(x)
					})
					.Where(x => x.Index >= 0)
					.ToDictionary(x => x.Name, x => x.Index);
			}

			public byte LanguageCode { get; }

			public int ColumnIndex(string name)
			{
				return Array.FindIndex(this.Fields, x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
			}

			public IEnumerable<object[]> GetRecords()
			{
				var rec = this.NextRecord();
				while (rec != null)
				{
					yield return rec;
					rec = this.NextRecord();
				}
			}

			public T Value<T>(object[] record, string key)
			{
				return this.indexes.TryGetValue(key.ToUpper(), out int index) ?
					(T)record[index] : default;
			}
		}
	}
}