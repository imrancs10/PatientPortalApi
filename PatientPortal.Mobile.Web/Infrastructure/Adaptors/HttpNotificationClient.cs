using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PatientPortal.Mobile.Web.Infrastructure.Adaptors
{
    public class HttpNotificationClient : IHttpNotificationClient
    {
        private const string _authorizationHeader = "Authorization";
        private readonly HttpClient httpClient;
        public HttpNotificationClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> SendPost<T>(T message, string uri, string secret) where T : new()
        {
            var callback = new Uri(uri, UriKind.Absolute);

            var content = new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, callback)
            {
                Content = content,
            };

            if (!string.IsNullOrEmpty(secret))
            {
                request.Headers.Add(_authorizationHeader, "Basic " + secret);
            }

            return await httpClient.SendAsync(request);
        }
    }
}