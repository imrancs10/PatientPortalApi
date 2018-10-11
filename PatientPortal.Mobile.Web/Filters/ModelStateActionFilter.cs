using Microsoft.AspNetCore.Mvc.Filters;
using PatientPortal.Mobile.Web.ActionResults;
using System.Threading.Tasks;

namespace PatientPortal.Mobile.Web.Filters
{
    public class ModelStateActionFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = new ModelValidationActionResult();
            }
            else
            {
                await next();
            }
        }
    }
}
