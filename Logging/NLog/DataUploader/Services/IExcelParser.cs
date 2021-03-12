using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataUploader.Services
{
    public interface IExcelParser
    {
        Task<string> Parse(IFormFile ifromFile, int subdivisionId);
    }
}