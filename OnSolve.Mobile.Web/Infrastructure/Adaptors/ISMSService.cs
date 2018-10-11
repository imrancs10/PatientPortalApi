using System.Net.Http;
using System.Threading.Tasks;

namespace OnSolve.Mobile.Web.Infrastructure.Adaptors
{
    public interface ISMSService
    {
        Task<HttpResponseMessage> SendMessage(string message, string phoneNumber);
    }
}