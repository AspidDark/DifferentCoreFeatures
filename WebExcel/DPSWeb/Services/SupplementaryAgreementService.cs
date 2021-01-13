using DPSWeb.Data;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;

namespace DPSWeb.Services
{
    public class SupplementaryAgreementService : ISupplementaryAgreementService
    {
        private readonly DataContext _dataContext;
        public SupplementaryAgreementService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public SupplementaryAgreement GetById(Guid id)
        {
            return _dataContext.SupplementaryAgreements.AsNoTracking().FirstOrDefault(x => x.Id == id);
        }


        public List<SupplementaryAgreement> GetByDate(int month, int year)
        {
            return _dataContext.SupplementaryAgreements
                .AsNoTracking().Where(x => x.GracePeriodStartDate.Month == month
            && x.GracePeriodStartDate.Year == year).ToList();
        }
    }
}
