using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ABSUploadClient.Entity.EntityModel
{
	public class PaymentOrderEntity
	{
		[Key]
		public Guid Id { get; set; }

		/// <summary>
		/// Дата поступления запроса
		/// </summary>
		[Column(TypeName = "date")]
		public DateTime RequestRecivedOn { get; set; }

		/// <summary>
		/// Номер кредитного договора для автоматической привязки Платёжного поручения 
		/// </summary>
		[MaxLength(100)]
		public string CreditContractNumber { get; set; }

		[MaxLength(100)]
		/// <summary> Номер  </summary>
		public string Number { get; set; }

		/// <summary> Дата поступления </summary>
		public DateTime IncomeDate { get; set; }

		/// <summary> Сумма </summary>
		[Column(TypeName = "decimal(18,2)"), Required]
		public decimal Amount { get; set; }

		/// <summary> Назначение платежа </summary>
		[MaxLength(1000)]
		public string Description { get; set; }

		/// <summary> Плательщик </summary>
		[MaxLength(1000)]
		public string PayerName { get; set; }

		/// <summary>
		/// Комментарий
		/// </summary>
		[MaxLength(200)]
		public string Comment { get; set; }

		/// <summary>
		/// Аутентификационная информация о пользователе Машина/Пользователь
		/// </summary>
		[MaxLength(500)]
		public string AuthentificationData { get; set; }
	}
}
