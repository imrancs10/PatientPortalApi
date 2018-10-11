using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using System.Net;

namespace PatientPortal.Mobile.Web.Models
{
    public enum ErrorCode
    {
        [HttpStatusCode(HttpStatusCode.OK)]
        [Description("No Error")]
        None,
        [HttpStatusCode(HttpStatusCode.Unauthorized)]
        [Description("The username or password you entered is incorrect. Please try again.")]
        InvalidUsernameOrPassword = 1002,

        [HttpStatusCode(HttpStatusCode.BadRequest)]
        [Description("User is invalid.")]
        InvalidUser = 1003,
    }

    public static class ErrorCodeExtensions
    {
        public static IActionResult Error(this ErrorCode errorCode)
        {
            ErrorCodeDetail errorCodeDetail = errorCode.GetDetail();
            return new ObjectResult(new { ErrorCode = errorCode, errorCodeDetail.Message })
            {
                StatusCode = (int)errorCodeDetail.StatusCode
            };
        }
        public static ErrorCodeDetail GetDetail(this ErrorCode errorCode)
        {
            return ResponseCodeCollection.ResponseCodeDetails[errorCode];
        }
        public static bool HasError(this ErrorCode errorCode)
        {
            return errorCode != ErrorCode.None;
        }
        
    }
}
