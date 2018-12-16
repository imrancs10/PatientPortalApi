using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace PatientPortalApi.Global
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
                    StatusCode = responseCodeType.GetAttributeOfType<HttpStatusCodeAttribute>().StatusCode,
                    ErrorCode = responseCodeType
                });

            }
        }

        public static Dictionary<ErrorCode, ErrorCodeDetail> ResponseCodeDetails { get; }

    }
}
