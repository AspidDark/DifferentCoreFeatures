using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DPSService.ContractDto
{
    public class FoVwMMKGraceContact_Dto
    {
		public Guid Id { get; set; }
		public DateTime CreatedOn { get; set; }
		public Guid CreatedById { get; set; }
		public DateTime ModifiedOn { get; set; }
		public Guid ModifiedById { get; set; }
		public int ProcessListeners { get; set; }
		public Guid ContactId { get; set; }
		public Guid ContractId { get; set; }
		public DateTime GracePeriodStartDate { get; set; }
		public DateTime GracePeriodEndDate { get; set; }
		public Guid DocGracePeriodId { get; set; }
		public bool GracePeriodStatus { get; set; }
		[MaxLength(250)]
		public string Note { get; set; }
		public DateTime DateGracePeriodDeclined { get; set; }
		public decimal PrincipalPayable { get; set; }
		public decimal PercentOdPayable { get; set; }
		public decimal PrincipalPzPayable { get; set; }
		public decimal PercentPzPayable { get; set; }
		public decimal PenaltyPzPayable { get; set; }
		public decimal StateDutyPayable { get; set; }
		public decimal TotalPayable { get; set; }
		[MaxLength(250)]
        public string BankingServiceType { get; set; }
        [MaxLength(250)]
        public string BankingServiceName { get; set; }
        public int LoanPeriodDaily { get; set; }
        public decimal LoanRateDay { get; set; }
        public int TermVariance { get; set; }
        public DateTime ContractSignedOn { get; set; }
		[MaxLength(250)]
		public string ContractNumber { get; set; }
		[MaxLength(250)]
		public string ContractNumberABS { get; set; }
	}
}
