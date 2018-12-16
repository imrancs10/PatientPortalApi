using System;
using System.ComponentModel;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PatientPortalApi.Global
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ErrorCode
    {
        [HttpStatusCode(HttpStatusCode.OK)]
        [Description("No Error")]
        None,
        [HttpStatusCode(HttpStatusCode.Unauthorized)]
        [Description("Registration Expired, Kindly renew it.")]
        RegistrationExpired = 1001,

        [HttpStatusCode(HttpStatusCode.BadRequest)]
        [Description("User is invalid.")]
        InvalidUser = 1002,

        [HttpStatusCode(HttpStatusCode.BadRequest)]
        [Description("You have reached the maximum attempt, your account is locked for a day.")]
        AccountLocked = 1003,

        [HttpStatusCode(HttpStatusCode.BadRequest)]
        [Description("Wrong Password, only 0 Attempt left, else your account will be locked for a day.")]
        AccountFailAttempt = 1004,

        [HttpStatusCode(HttpStatusCode.BadRequest)]
        [Description("Please Enter correct Mobile Number")]
        InCorrectMobileNumber = 1005,

        [HttpStatusCode(HttpStatusCode.BadRequest)]
        [Description("Please Enter correct Email Id")]
        InCorrectEmailId = 1006,

        [HttpStatusCode(HttpStatusCode.BadRequest)]
        [Description("Mobile Number or Email Id already in our database, kindly chhange it or reset your account.")]
        MobileOrEmailExists = 1007,

        [HttpStatusCode(HttpStatusCode.BadRequest)]
        [Description("User is already register.")]
        UserAlreadyExists = 1008,



    }

    public static class ErrorCodeExtensions
    {
        //public static IActionResult Error(this ErrorCode errorCode)
        //{
        //    ErrorCodeDetail errorCodeDetail = errorCode.GetDetail();
        //    return new ObjectResult(new { ErrorCode = errorCode, errorCodeDetail.Message })
        //    {
        //        StatusCode = (int)errorCodeDetail.StatusCode
        //    };
        //}
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
