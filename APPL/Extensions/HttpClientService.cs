
using APPL.Extensions;
using System.Net.Http.Headers;


namespace APPL.Extations
{
    public class HttpClientService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        //private readonly LocalStoregeService _localStoregeService;

        public HttpClientService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            //_localStoregeService = localStoregeService;
        }

        // Method to create a default HttpClient
        private HttpClient CreateClient() => _httpClientFactory!.CreateClient(Constant.HttpClientName);

        // Public client without authorization header
        public HttpClient GetPublicClient() => CreateClient();

        // Private client with authorization header
        public async Task<HttpClient> GetPrivateClient()
        {
            try
            {
                var client = CreateClient();

                // Retrieve the token information from LocalStorage
                //var localStorageDTO = await _localStoregeService.GetModelFromToken();

                //if (string.IsNullOrEmpty(localStorageDTO.Token))
                //{
                //    // Return client without Authorization header if token is not available
                //    return client;
                //}

                // Add Authorization header with the token
                //client.DefaultRequestHeaders.Authorization =
                //    new AuthenticationHeaderValue(Constant.HttpClientHeaderScheme, localStorageDTO.Token);

                return client;
            }
            catch
            {
                // Return a default client without Authorization in case of failure
                return new HttpClient();
            }
        }
    }
}
