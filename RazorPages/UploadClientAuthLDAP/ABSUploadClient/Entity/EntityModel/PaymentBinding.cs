using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ABSUploadClient.Entity.EntityModel
{
	public class PaymentBinding
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[Column(TypeName = "bigint")]
		public long PaymentId { get; set; }

		[Column(TypeName = "bigint")]
		public long LoanId { get; set; }

		[Column(TypeName = "int")]
		public BindingStatus Status { get; set; }

		[Column(TypeName = "datetime")]
		public DateTime Date { get; set; }

		/// <summary>
		/// Статус привязки
		/// </summary>
		public enum BindingStatus : int
		{
			/// <summary>
			/// Запланирована
			/// </summary>
			Scheduled,

			/// <summary>
			/// Обрабатывается
			/// </summary>
			BeingProcessed,

			/// <summary>
			/// Обработана успешно
			/// </summary>
			ProcessedSuccessfully,

			/// <summary>
			/// Обработана с ошибкой
			/// </summary>
			ProccessedWithError
		}
		/// <summary>
		/// Id в таблице PaymentOrder
		/// </summary>
		public Guid PaymentOrderId { get; set; }
		/// <summary>
		/// текст ошибки в результате загрузки
		/// </summary>
		public string BindingErrorMessage { get; set; }
	}
}