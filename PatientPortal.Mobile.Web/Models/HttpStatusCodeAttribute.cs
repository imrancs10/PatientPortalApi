using System;
using System.Net;

namespace PatientPortal.Mobile.Web.Models
{
    public class HttpStatusCodeAttribute : Attribute
    {
        public HttpStatusCodeAttribute(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }

        public HttpStatusCode StatusCode { get; }

    }
}
