using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;
using Zaghloul.QA.TestRail.xUnit.Config;
using Zaghloul.QA.TestRail.xUnit.TestRail.Enums;
using Zaghloul.QA.TestRail.xUnit.TestRail.Helpers;

namespace Zaghloul.QA.TestRail.xUnit.TestRail.Client
{
    public class BaseClient
    {
        private readonly HttpClient _httpClient = new(new HttpClientHandler { AllowAutoRedirect = true });
        private string BaseURL;
        private string UserName;
        private string ApiKeyOrPassword;

        public BaseClient()
        {
            var config = AppConfigHelper.GetAppConfigurations();
            UserName = config.Email;
            ApiKeyOrPassword = config.ApiKey;
            BaseURL = config.Url;
        }

        protected string CreateUri(RequestType requestType, RequestTarget target, ulong? id1 = null)
        {
            var type = requestType.GetStringValue();
            var targetName = target.GetStringValue();
            return $"?/api/v2/{type}_{targetName}{(id1.HasValue ? "/" + id1.Value : string.Empty)}";
        }

        protected async Task<T> Post<T>(string uri, object data = null) => await SendRequest<T>(uri, HttpMethod.Post, data);

        protected async Task<T> Get<T>(string uri) => await SendRequest<T>(uri, HttpMethod.Get);

        private async Task<T> SendRequest<T>(string uri, HttpMethod method, object data = null)
        {
            string responseFromServer = null;

            var request = CreateRequest($"{BaseURL}{uri}", method, data);
            var response = await _httpClient.SendAsync(request);

            responseFromServer = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var message = $"[xUnit-TestRail-Plugin] Request failed with status code {response.StatusCode}: {responseFromServer}";
                Debug.WriteLine(message);
                throw new HttpRequestException(message);
            }

            return JsonConvert.DeserializeObject<T>(responseFromServer);
        }

        private HttpRequestMessage CreateRequest(string url, HttpMethod method, object data)
        {
            var authInfo = Convert.ToBase64String(Encoding.Default.GetBytes($"{UserName}:{ApiKeyOrPassword}"));
            var request = new HttpRequestMessage(method, url);
            request.Headers.Add("User-Agent", "Zaghloul xUnit TestRail .NET Client");
            request.Headers.Add("Authorization", $"Basic {authInfo}");

            if (data != null)
            {
                var jsonData = JsonConvert.SerializeObject(data);
                request.Content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            }
            else
            {
                request.Content = new StringContent("{}", Encoding.UTF8, "application/json");
            }

            return request;
        }
    }
}