namespace ClientUI
{
    public class ApiService
    {
        private readonly AppSettings _appSettings;

        public ApiService(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public string GetApiBaseUrl()
        {
            return _appSettings.ApiBaseUrl;
        }
    }
}
