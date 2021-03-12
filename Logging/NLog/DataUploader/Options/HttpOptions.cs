namespace DataUploader.Options
{
    public class HttpOptions
    {
        public string BasePath { get; set; }
        public string UploadPath { get; set; }
        public string AuthorizePath { get; set; }
        public string AuthLogin { get; set; }
        public string Password { get; set; }
        public int Timeout { get; set; }
    }
}
