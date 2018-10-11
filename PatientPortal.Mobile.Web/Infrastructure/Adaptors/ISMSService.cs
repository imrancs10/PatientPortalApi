using System.Net.Http;
using System.Threading.Tasks;

namespace PatientPortal.Mobile.Web.Infrastructure.Adaptors
{
    public interface ISMSService
    {
        Task<HttpResponseMessage> SendMessage(string message, string phoneNumber);
    }
}