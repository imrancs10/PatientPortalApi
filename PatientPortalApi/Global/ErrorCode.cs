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

        [HttpStatusCode(HttpStatusCode.BadRequest)]
        [Description("Data is already register.")]
        DataAlreadyExist = 1009,

        [HttpStatusCode(HttpStatusCode.BadRequest)]
        [Description("No Doctor Available for the given department.")]
        NoDoctorFound = 1010,

        [HttpStatusCode(HttpStatusCode.BadRequest)]
        [Description("No Doctor Schedule Available.")]
        NoDoctorScheduleFound = 1011,

        [HttpStatusCode(HttpStatusCode.BadRequest)]
        [Description("Only Limited Appointment can be booked for a day.")]
        OnlyLimitedAppointmentCanBooked = 1012,

        [HttpStatusCode(HttpStatusCode.BadRequest)]
        [Description("this pin already used for this device, choose another.")]
        SamePinUse = 1013,

        [HttpStatusCode(HttpStatusCode.BadRequest)]
        [Description("Mobile number Or Email is not Correct.")]
        MobileEmailInCorrect = 1014,

        [HttpStatusCode(HttpStatusCode.BadRequest)]
        [Description("Appointment has not been cancelled.")]
        AppointmentNoCanceled = 1015,

        [HttpStatusCode(HttpStatusCode.BadRequest)]
        [Description("Registration or Mobile number is not Correct.")]
        RegistrationorMobileInCorrect = 1016,

        [HttpStatusCode(HttpStatusCode.BadRequest)]
        [Description("password must be at least 8 characters long.")]
        Password8CharecterRequired = 1017,

        [HttpStatusCode(HttpStatusCode.BadRequest)]
        [Description("password must be at least 1 Upper Case characters.")]
        PasswordUpperCharecterRequired = 1018,

        [HttpStatusCode(HttpStatusCode.BadRequest)]
        [Description("password must be at least 1 Numeric characters.")]
        PasswordNumericCharecterRequired = 1019,

        [HttpStatusCode(HttpStatusCode.BadRequest)]
        [Description("password must be at least 1 Special characters.")]
        PasswordSpecialCharecterRequired = 1020,

        [HttpStatusCode(HttpStatusCode.BadRequest)]
        [Description("CR Number is already exist in our database, choose another one.")]
        CRNumberExists = 1021,

        [HttpStatusCode(HttpStatusCode.BadRequest)]
        [Description("Email Id is already used for other account.")]
        EmailIdExists = 1022,

        [HttpStatusCode(HttpStatusCode.BadRequest)]
        [Description("CR Number not found or expire, Kindly contact to hospital.")]
        CRNumberNotFoundOrExpire = 1023,

        [HttpStatusCode(HttpStatusCode.BadRequest)]
        [Description("Please Enter correct Email Address.")]
        WrongEmailAddress = 1024,

        [HttpStatusCode(HttpStatusCode.BadRequest)]
        [Description("Patient is already register with same Mobile Number or EmailId.")]
        EmailOrMobileNoExists = 1025,

        [HttpStatusCode(HttpStatusCode.BadRequest)]
        [Description("User is not saved, might be of Email Id or Mobile No is already taken.")]
        UserNotSaved = 1026,

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
