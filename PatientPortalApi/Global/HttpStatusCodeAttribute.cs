using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace PatientPortalApi.Global
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