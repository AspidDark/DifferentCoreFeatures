using RestSharp;

namespace DataUploader.CascadAuthirization
{
    public interface ICascadClientFactory
    {
        string GetAuthCookies();
        IRestResponse PostExcelFile(string json);
    }
}