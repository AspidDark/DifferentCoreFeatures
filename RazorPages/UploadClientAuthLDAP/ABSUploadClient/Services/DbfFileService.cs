using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DotNetDBF;
using UploadClient.Models.AbsDTO;

namespace ABSUploadClient.Services
{
	public partial class DbfFileService : IFileService, IDisposable
	{
		private const int MAXLEN = 254;

		private readonly string path;
		private readonly int statColumnIndex, bindColumnIndex;

		private readonly DbfWriter writer;

		public DbfFileService(string path, string statusColumnName, string bindColumnName)
		{
			this.path = path;

			using var stream = new FileStream(path, FileMode.Open);
			using var reader = new DbfReader(stream);

			int ncols = reader.Fields.Length;
			var fields = new List<DBFField>(reader.Fields);

			this.statColumnIndex = reader.ColumnIndex(statusColumnName);
			if (this.statColumnIndex < 0)
			{
				this.statColumnIndex = ncols++;
				fields.Add(new DBFField(statusColumnName, NativeDbType.Char, MAXLEN));
			}

			this.bindColumnIndex = reader.ColumnIndex(bindColumnName);
			if (this.bindColumnIndex < 0)
			{
				this.bindColumnIndex = ncols++;
				fields.Add(new DBFField(bindColumnName, NativeDbType.Char, MAXLEN));
			}

			this.writer =
				new DbfWriter(fields.ToArray(), reader.CharEncoding, reader.LanguageCode);
		}

		public IEnumerable<PaymentOrder> GetPaymentOrders()
		{
			using var stream = new FileStream(this.path, FileMode.Open);
			using var reader = new DbfReader(stream);

			return reader.GetRecords()
				.Where(x => "да".Equals(reader.Value<string>(x, "elcheck")))
				.Select(x =>
				{
					var tag = x;
					if (x.Length < this.writer.Fields.Length)
					{
						tag = new object[this.writer.Fields.Length];
						x.CopyTo(tag, 0);
					};
					return new PaymentOrder
					{
						CreditContractNumber = ExtractContractNumber(reader.Value<string>(x, "msg")),
						Number = reader.Value<decimal>(x, "acceptnum").ToString(),
						IncomeDate = reader.Value<DateTime>(x, "acceptdate"),
						Amount = reader.Value<decimal>(x, "paysum"),
						Description = reader.Value<string>(x, "msg"),
						PayerName = reader.Value<string>(x, "fromname"),
						Tag = tag
					};
				})
				.ToArray();
		}

		public void SetBindingResult(PaymentOrder order, string status, string result)
		{
			var rec = (object[])order.Tag;
			rec[this.statColumnIndex] = status?.Substring(0, Math.Min(status.Length, MAXLEN));
			rec[this.bindColumnIndex] = result?.Substring(0, Math.Min(result.Length, MAXLEN));
		}

		public void Save(IEnumerable<PaymentOrder> orders)
		{
			var records = orders
				.Select(x => (object[])x.Tag);

			foreach (var rec in records)
				this.writer.AddRecord(rec);

			using var stream = new FileStream(this.path, FileMode.Open);
			this.writer.Write(stream);
			this.writer.Close();
		}

		public void Dispose()
		{
			this.writer.Close();
		}

		private static string ExtractContractNumber(string txt)
		{
			var match = Regex.Match(txt, @".*?(?'num'\d{1,}).*?");
			return match.Success ?
				match.Groups["num"].Value : null;
		}
	}
}