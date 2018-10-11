using System.Net;

namespace OnSolve.Mobile.Web.Models
{
    public class ErrorCodeDetail
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
    }
}
