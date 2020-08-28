using ABSUploadClient.Entity;
using ABSUploadClient.Entity.EntityModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ABSUploadClient.Services
{
    public class PaymentBindingService : IPaymentBindingService
    {
        private readonly PaymentOrdersContext _dbContext;
        public PaymentBindingService(PaymentOrdersContext dbContext)
        {
            _dbContext = dbContext;
        }

        public PaymentBinding GetPaymentBindingByPaymentOrderId(Guid paymentOrdeId)
        {
            return _dbContext.PaymentBindings
                .AsNoTracking()
                .FirstOrDefault(x => x.PaymentOrderId == paymentOrdeId);
        }
    }
}
