using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace PatientPortal.Mobile.Web.ActionResults
{
    public class ModelValidationActionResult : IActionResult
    {

        public async Task ExecuteResultAsync(ActionContext context)
        {
            var errors = context.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            var objectResult = new ObjectResult(new { Errors = errors })
            {
                StatusCode = StatusCodes.Status400BadRequest
            };

            await objectResult.ExecuteResultAsync(context);
        }
    }
}
