using System.Collections.Generic;
using UploadClient.Models.AbsDTO;

namespace ABSUploadClient.Services
{
	public interface IFileService
	{
		public IEnumerable<PaymentOrder> GetPaymentOrders();

		public void SetBindingResult(PaymentOrder order, string status, string result);

		public void Save(IEnumerable<PaymentOrder> orders);
	}
}