using System.Net.Http.Headers;
using System.Text;

namespace rtdc_rest.api.Helpers
{
    public class HttpClientHelper
    {
        private readonly IConfiguration _configuration;

        public HttpClientHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public HttpClientHelper()
        {
            
        }
        public string SendPOSTRequest(string userName, string password, string endPoint, string postData)
         {
            string apiUrl = _configuration.GetSection("AppSettings:ApiUrl").Value;
            string urlPathForRequest = apiUrl.ToString();

            urlPathForRequest = urlPathForRequest + endPoint;

            var authenticationString = $"{userName}:{password}";
            var base64String = Convert.ToBase64String(Encoding.ASCII.GetBytes(authenticationString));

            var content = new StringContent(postData, Encoding.UTF8, "application/json");

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, urlPathForRequest);
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64String);
            requestMessage.Content = content;

            var httpClient = new HttpClient();
            
            var response = httpClient.Send(requestMessage);

            return response.Content.ReadAsStringAsync().Result;

        }

    }
}
