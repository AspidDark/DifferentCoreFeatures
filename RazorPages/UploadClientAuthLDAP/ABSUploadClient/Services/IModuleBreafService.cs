using System.Collections.Generic;
using System.Threading.Tasks;
using ABSUploadClient.Dto;

namespace ABSUploadClient.Services
{
    public interface IModuleBreafService
    {
        IEnumerable<ModuleBriefDto> GetAllModuleBreaf();
    }
}