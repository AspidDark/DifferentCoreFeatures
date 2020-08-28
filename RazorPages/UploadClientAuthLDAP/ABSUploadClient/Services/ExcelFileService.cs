using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ClosedXML.Excel;
using UploadClient.Models.AbsDTO;

namespace ABSUploadClient.Services
{
	public class ExcelFileService : IFileService
	{
		private readonly IXLWorkbook workbook;

		private readonly IXLTable table;
		private readonly int statColumnIndex, bindColumnIndex;

		public ExcelFileService(IXLWorkbook workbook, string worksheetName, string statusColumnName, string bindColumnName)
		{
			this.workbook = workbook;

			var ws = workbook.Worksheet(worksheetName);
			if (ws == null)
				throw new Exception("В файле Excel не найден лист " + worksheetName);
			this.table = ws.Tables.FirstOrDefault();

			var headerRow = this.table.RowsUsed()
				.First();
			if (headerRow == null)
				throw new Exception("В файле Excel не найдена строка заголовка");

			var cellStat = headerRow
				.CellsUsed()
				.FirstOrDefault(x => x.GetValue<string>() == statusColumnName);

			var cellBind = headerRow
				.CellsUsed()
				.FirstOrDefault(x => x.GetValue<string>() == bindColumnName);

			if (cellStat == null)
			{
				this.statColumnIndex = headerRow.CellsUsed().Count() + 1;
				cellStat = headerRow.Cell(this.statColumnIndex);
				cellStat.SetValue(statusColumnName);

				this.bindColumnIndex = headerRow.CellsUsed().Count() + 2;
				cellBind = headerRow.Cell(this.bindColumnIndex);
				cellBind.SetValue(bindColumnName);

				this.workbook.Save();
			}
			else
			{
				this.statColumnIndex = cellStat.Address.ColumnNumber;
				this.bindColumnIndex = cellBind.Address.ColumnNumber;
			}
		}

		/// <summary>
		/// Извлекает записи платежей
		/// </summary>
		public IEnumerable<PaymentOrder> GetPaymentOrders()
		{
			int nline = 1;

			int n = this.table.RowsUsed().Count();
			var result = new List<PaymentOrder>(n);

			foreach (var row in this.table.RowsUsed().Skip(1))
			{
				nline++;
				var order = new PaymentOrder
				{
					CreditContractNumber = row.Cell(1).GetString(),
					Number = row.Cell(2).GetString(),
					IncomeDate = DateTime.ParseExact(row.Cell(3).GetString(), "dd.MM.yyyy",
						CultureInfo.InvariantCulture),
					Amount = Convert.ToDecimal(row.Cell(4)
					 .GetString()
					 .Replace(" ", "")
					 .Replace(',', '.'), CultureInfo.InvariantCulture),
					Description = row.Cell(5).GetString(),
					PayerName = row.Cell(6).GetString(),
					Tag = row
				};
				result.Add(order);
			}

			return result;
		}

		/// <summary>
		/// Записывает результат привязки
		/// </summary>
		public void SetBindingResult(PaymentOrder paymentOrder, string status, string result)
		{
			var row = (IXLRangeRow)paymentOrder.Tag;
			row.Cell(this.statColumnIndex).SetValue(status);
			row.Cell(this.bindColumnIndex).SetValue(result);
		}

		/// <summary>
		/// Сохраняет документ
		/// </summary>
		public void Save(IEnumerable<PaymentOrder> orders)
		{
			this.workbook.Save();
		}
	}
}