namespace ClientUI
{
    public class AppSettings
    {
        private readonly string _apiBaseUrl;
        public AppSettings(IConfiguration configuration)
        {
            _apiBaseUrl = configuration["AppSettings:ApiBaseUrl"];
        }
        public string ApiBaseUrl { get; set; } = string.Empty;
        public AppSettings() { }
        public string GetApiBaseUrl()
        {
            return _apiBaseUrl;
        }
    }
}
