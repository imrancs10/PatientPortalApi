using Microsoft.Extensions.Configuration;
using OnSolve.Mobile.Web.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace OnSolve.Mobile.Web.Infrastructure.Adaptors
{
    public class SMSService : ISMSService
    {
        private const string _smsServiceConfiguration = "SMSService";
        private const string _secretKey = "TbcXuUsuuVPkSeRRV7zfjPLRcN78K7te9v3t2jLkQbvj2b2yTrD362vTrcSLaaQUvvXmDzg6mCeXq6nU8BMPdTyuVfQV2xF8Ruj6DcMAESqEznqhNNWSxmWhsDQ5L73t";

        public SMSService(IConfiguration configuration, IHttpNotificationClient httpclient)
        {
            Configuration = configuration;
            HttpClient = httpclient;
        }
        private IConfiguration Configuration { get; }
        private IHttpNotificationClient HttpClient { get; }
        public async Task<HttpResponseMessage> SendMessage(string message, string phoneNumber)
        {
            var smsmessage = new SMSMessageModel() { Body = message, Address = phoneNumber };
            return await HttpClient.SendPost(smsmessage
                 , Configuration[_smsServiceConfiguration]
                  , GenerateAutherizationToken());

        }
        private string GenerateAutherizationToken()
        {

            // Logic to be implemented when we get Actual service
            return _secretKey;
        }
    }
}
