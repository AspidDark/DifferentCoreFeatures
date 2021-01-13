using DPSWeb.Models;

namespace DPSWeb.Reports
{
    public interface ISupplementaryAgreementReport
    {
        FileModel GetReport(int month, int year);
    }
}