using System;
using System.ComponentModel.DataAnnotations;

namespace DPSService.DB
{
    public class GraceContact : BaseEntity
    {
		public Guid ContractId { get; set; } 
		public DateTime GracePeriodStartDate { get; set; } 
		public bool GracePeriodStatus { get; set; } 
		public decimal PrincipalPayable { get; set; } 
		[MaxLength(250)]
		public string BankingServiceType { get; set; } 
		[MaxLength(250)]
		public string BankingServiceName { get; set; } 
		public int LoanPeriodDaily { get; set; } 
		public decimal LoanRateDay { get; set; } 
		public int TermVariance { get; set; } 
		public DateTime ContractSignedOn { get; set; } 

		public string ContractNumber { get; set; }

		public DateTime UpdateDTTM { get; set; }

		public DateTime CreatedOn { get; set; }

		public DateTime GracePeriodEndDate { get; set; }
		/// <summary>
		/// Номер договора в системе АБС
		/// </summary>
		[MaxLength(50)]
		public string ContractNumberABS { get; set; }

		//Данные для калькулятора Версии 2. Миграция не проведена
		public decimal PrincipalPzPayable { get; set; }
		public decimal PaymentAmount { get; set; }
		public decimal PercentPzPayable { get; set; }
		public decimal PenaltyPzPayable { get; set; }
		public int ContractStatus { get; set; }
		public decimal TotalPercentPzPaid { get; set; }
		public decimal Balance { get; set; }
		//3
		public decimal TotalPercentCharge { get; set; }
		public decimal TotalPercentPzPayable { get; set; }
		/// <summary>
		/// Текущая дата закрытия займа по договору Без каникул
		/// </summary>
		public DateTime OldDateClosedContract { get; set; }
		/// <summary>
		/// Сумма оплаченных пени
		/// </summary>
		public decimal PaymentPenalty { get; set; }
		/// <summary>
		/// Задолженность по просроченным процентам
		/// </summary>
		public decimal PercentPzPayableCollection { get; set; }
		/// <summary>
		/// Задолженность по просроченному ОД
		/// </summary>
		public decimal PrincipalPzPayableCollection { get; set; }
	}
}
