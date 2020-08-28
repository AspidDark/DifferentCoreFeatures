using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ABSUploadClient.Dto;
using ABSUploadClient.Entity;

namespace ABSUploadClient.Services
{
    public class ModuleBreafService : IModuleBreafService
    {
        private readonly PaymentOrdersContext _dbContext;

        public ModuleBreafService(PaymentOrdersContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<ModuleBriefDto> GetAllModuleBreaf()
        {
            var response = _dbContext.ModuleBriefs.AsNoTracking()
                .Select(m => new ModuleBriefDto { Accout = m.Accout, ModuleValue = m.ModuleValue });
            return response;
        }
    }
}
