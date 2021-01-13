using DPSWeb.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DPSWeb.Services
{
    public class InterestChargeService : IInterestChargeService
    {
        private readonly DataContext _dataContext;
        public InterestChargeService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public InterestCharge GetById(Guid id)
        {
            return _dataContext.InterestCharges.AsNoTracking().FirstOrDefault(x => x.Id == id);
        }

        public List<InterestCharge> GetByDate(int month, int year)
        {
            return _dataContext.InterestCharges
                .AsNoTracking().Where(x => x.LastDayOfMonth.Month == month
            && x.LastDayOfMonth.Year == year).ToList();
        }

    }
}
