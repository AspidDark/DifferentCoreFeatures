using ABSUploadClient.Entity.EntityModel;
using System;

namespace ABSUploadClient.Services
{
    public interface IPaymentBindingService
    {
        PaymentBinding GetPaymentBindingByPaymentOrderId(Guid paymentOrdeId);
    }
}