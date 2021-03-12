using DataUploader.Options;
using RestSharp;

namespace DataUploader.CascadAuthirization
{
    public class CascadClientFactory : ICascadClientFactory
    {
        private readonly HttpOptions _httpOptions;
        public CascadClientFactory(HttpOptions httpOptions)
        {
            _httpOptions = httpOptions;
        }

        public string GetAuthCookies()
        {
            var client = new RestClient($"{_httpOptions.BasePath}{_httpOptions.AuthorizePath}");
            client.Timeout = _httpOptions.Timeout;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Pragma", "no-cache");
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("sec-ch-ua", "\"Chromium\";v=\"88\", \"Google Chrome\";v=\"88\", \";Not A Brand\";v=\"99\"");
            request.AddHeader("sec-ch-ua-mobile", "?0");
            request.AddHeader("Upgrade-Insecure-Requests", "1");
            request.AddHeader("Origin", _httpOptions.BasePath);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
            request.AddHeader("Sec-Fetch-Site", "same-origin");
            request.AddHeader("Sec-Fetch-Mode", "navigate");
            request.AddHeader("Sec-Fetch-User", "?1");
            request.AddHeader("Sec-Fetch-Dest", "document");
            request.AddHeader("Referer", $"{_httpOptions.BasePath}/login/?ReturnUrl=%2fHome%2fIndex");
            request.AddHeader("Accept-Language", "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7");
            request.AddHeader("Cookie", "__AUTH_COOKIE=");
            request.AddParameter("login", _httpOptions.AuthLogin);
            request.AddParameter("password", _httpOptions.Password);

            IRestResponse response = client.Execute(request);

            return response.Cookies[0].Name + response.Cookies[0].Value;
        }

        public IRestResponse PostExcelFile(string json)
        {
            string authCoookies = GetAuthCookies();

            var client = new RestClient($"{_httpOptions.BasePath}{_httpOptions.UploadPath}");
            client.Timeout = _httpOptions.Timeout;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Pragma", "no-cache");
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("sec-ch-ua", "\"Chromium\";v=\"88\", \"Google Chrome\";v=\"88\", \";Not A Brand\";v=\"99\"");
            request.AddHeader("X-Requested-With", "XMLHttpRequest");
            request.AddHeader("sec-ch-ua-mobile", "?0");
            //client.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/88.0.4324.190 Safari/537.36";
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "*/*");
            request.AddHeader("Origin", _httpOptions.BasePath);
            request.AddHeader("Sec-Fetch-Site", "same-origin");
            request.AddHeader("Sec-Fetch-Mode", "cors");
            request.AddHeader("Sec-Fetch-Dest", "empty");
            request.AddHeader("Referer", _httpOptions.BasePath);
            request.AddHeader("Accept-Language", "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7");
            request.AddHeader("Cookie", authCoookies);
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return response;
        }
    }
}
