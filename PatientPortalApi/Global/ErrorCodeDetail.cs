using System.Net;

namespace PatientPortalApi.Global
{
    public class ErrorCodeDetail
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
        public ErrorCode ErrorCode { get; set; }
    }
}
