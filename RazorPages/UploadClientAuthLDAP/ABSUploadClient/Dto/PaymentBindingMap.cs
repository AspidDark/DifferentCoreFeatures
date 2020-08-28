using System;
using UploadClient.Models.AbsDTO;

namespace ABSUploadClient.Dto
{
    public class PaymentBindingMap
    {
       public  PaymentOrder PaymentOrder { get; set; }

        public Guid PaymentOrderId { get; set; }
    }
}
