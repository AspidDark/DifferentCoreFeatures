using DPSWeb.Data;
using System;
using System.Collections.Generic;

namespace DPSWeb.Services
{
    public interface IInterestChargeService
    {
        InterestCharge GetById(Guid id);
        List<InterestCharge> GetByDate(int month, int year);
    }
}