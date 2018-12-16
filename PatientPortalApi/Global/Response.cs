using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PatientPortalApi.Global
{
    public class Response<T>
    {
        public ErrorCodeDetail ErrorCode { get; set; }
        public IEnumerable<T> ResponseData { get; set; }

        public Response(ErrorCodeDetail errorCode, IEnumerable<T> data)
        {
            ErrorCode = errorCode;
            ResponseData = data;
        }
    }
}