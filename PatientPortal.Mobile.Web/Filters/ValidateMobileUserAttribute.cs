using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PatientPortal.Mobile.Data;
using System;
using System.Threading.Tasks;
using System.Collections;
using System.Linq;
using PatientPortal.Mobile.Web.Models;

namespace PatientPortal.Mobile.Web.Filters
{
    public class ValidMobileUserAttribute : TypeFilterAttribute
    {
        public ValidMobileUserAttribute() : base(typeof(ValidateUserIdAttribute))
        { }
        private class ValidateUserIdAttribute : ActionFilterAttribute
        {
            private readonly Func<PatientPortalMobileContext> _dbProvider;

            public ValidateUserIdAttribute(Func<PatientPortalMobileContext> dbprovider)
            {
                _dbProvider = dbprovider;
            }

            public override async Task OnActionExecutionAsync(ActionExecutingContext context,
                            ActionExecutionDelegate next)
            {
                if (IsUserValid(context))
                {
                    await next();
                }
                else
                {
                    SetContextResult(context);
                }
            }

            private void SetContextResult(ActionExecutingContext context)
            {
                ErrorCodeDetail errorDetail = ResponseCodeCollection.ResponseCodeDetails[ErrorCode.InvalidUser];
                context.Result = new ObjectResult(new
                {
                    ErrorCode = ErrorCode.InvalidUser,
                    errorDetail.Message
                })
                { StatusCode = (int)errorDetail.StatusCode };
            }

            private bool IsUserValid(ActionExecutingContext context)
            {
                bool isUserValid = false;
                string username = Convert.ToString(context.RouteData.Values["username"]);
                if (username != null)
                {
                    using (var dbContext = _dbProvider())
                    {
                        MobileUser user = dbContext.MobileUser.Where(c => c.Username == username).FirstOrDefault();
                        isUserValid = user != null;
                    }
                }
                return isUserValid;
            }

        }
    }
}