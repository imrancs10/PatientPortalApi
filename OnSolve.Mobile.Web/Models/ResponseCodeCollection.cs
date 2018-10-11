using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace OnSolve.Mobile.Web.Models
{
    public static class ResponseCodeCollection
    {

        static ResponseCodeCollection()
        {
            ResponseCodeDetails = new Dictionary<ErrorCode, ErrorCodeDetail>();
            foreach (ErrorCode responseCodeType in Enum.GetValues(typeof(ErrorCode)))
            {
                ResponseCodeDetails.Add(responseCodeType, new ErrorCodeDetail()
                {
                    Message = responseCodeType.GetAttributeOfType<DescriptionAttribute>().Description,
                    StatusCode = responseCodeType.GetAttributeOfType<HttpStatusCodeAttribute>().StatusCode
                });

            }
        }

        public static Dictionary<ErrorCode, ErrorCodeDetail> ResponseCodeDetails { get; }

    }
}
