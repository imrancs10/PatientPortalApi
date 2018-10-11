using Microsoft.AspNetCore.Mvc;
using PatientPortal.Mobile.Web.Models;
using System;

namespace PatientPortal.Mobile.Web
{
    public class Errorable<T>
    {
        public readonly T Value;

        public readonly ErrorCode ErrorCode;

        public Errorable(T value)
        {
            Value = value;            
        }

        public Errorable(ErrorCode errorCode)
        {
            ErrorCode = errorCode;
        }

        public bool HasValue => Value != null;

        public bool HasError => !HasValue;
        public IActionResult Error()
        {
            ErrorCodeDetail errorCodeDetail = ErrorCode.GetDetail();
            return new ObjectResult(new { ErrorCode, errorCodeDetail.Message })
            {
                StatusCode = (int)errorCodeDetail.StatusCode
            };
        }
    }

    public static class ErrorableExtensions
    {
        public static IActionResult BuildResult<T>(this Errorable<T> errorable,
                                                     Func<T, IActionResult> success)
        {
            return errorable.HasValue ? success(errorable.Value) : errorable.ErrorCode.Error();
        }        
    }    
}
