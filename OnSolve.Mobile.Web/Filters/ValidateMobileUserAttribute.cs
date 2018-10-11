using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OnSolve.Mobile.Data;
using OnSolve.Mobile.Data.Entites;
using System;
using System.Threading.Tasks;
using System.Collections;
using System.Linq;
using OnSolve.Mobile.Web.Models;

namespace OnSolve.Mobile.Web.Filters
{
    public class ValidMobileUserAttribute : TypeFilterAttribute
    {
        public ValidMobileUserAttribute() : base(typeof(ValidateUserIdAttribute))
        { }
        private class ValidateUserIdAttribute : ActionFilterAttribute
        {
            private readonly Func<OnSolveMobileContext> _dbProvider;

            public ValidateUserIdAttribute(Func<OnSolveMobileContext> dbprovider)
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