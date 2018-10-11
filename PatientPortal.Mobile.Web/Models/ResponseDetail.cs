using System.Net;

namespace PatientPortal.Mobile.Web.Models
{
    public class ErrorCodeDetail
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
    }
}
