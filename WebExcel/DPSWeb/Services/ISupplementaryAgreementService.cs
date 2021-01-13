using DPSWeb.Data;
using System;
using System.Collections.Generic;

namespace DPSWeb.Services
{
    public interface ISupplementaryAgreementService
    {
        SupplementaryAgreement GetById(Guid id);

        List<SupplementaryAgreement> GetByDate(int month, int year);
    }
}