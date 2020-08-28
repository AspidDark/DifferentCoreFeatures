using System;

namespace UploadClient.Models.AbsDTO
{
	public class PaymentOrderLinks
	{
		public long PaymentId { get; set; }

		public long CreditContractId { get; set; }

		public PaymentOrder PaymentOrder { get; set; }

		public Guid PaymentOrderId { get; set; }
	}
}