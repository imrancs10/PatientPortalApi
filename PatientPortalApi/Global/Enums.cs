using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Web;

namespace PatientPortalApi.Global
{
    public static class Enums
    {
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
            [Description("Wrong Password, only #1004 Attempt left, else your account will be locked for a day.")]
            AccountFailAttempt = 1004,
        }
        public enum LoginMessage
        {
            Authenticated = 1,
            InvalidCreadential = 2,
            LoginFailed,
            UserDeleted,
            UserInactive,
            UserBlocked,
            NoResponse
        }

        public enum CrudStatus
        {
            Saved = 1,
            NotSaved = 2,
            Updated,
            NotUpdated,
            Deleted,
            NotDeleted,
            DataNotFound,
            DataAlreadyExist,
            SessionExpired,
            InvalidPostedData,
            InvalidPastDate,
            InternalError,
            RegistrationExpired
        }

        public enum ReportType
        {
            Bill,
            Lab
        }

        public enum JsonResult
        {
            Data_NotFound = 100,
            Invalid_DataId = 101,
            Data_Expire = 102,
            Success = 103,
            Unsuccessful
        }
    }
}