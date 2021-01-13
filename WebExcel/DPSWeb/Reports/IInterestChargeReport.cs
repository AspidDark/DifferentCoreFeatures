using DPSWeb.Models;

namespace DPSWeb.Reports
{
    public interface IInterestChargeReport
    {
        FileModel GetReport(int month, int yerar);
    }
}