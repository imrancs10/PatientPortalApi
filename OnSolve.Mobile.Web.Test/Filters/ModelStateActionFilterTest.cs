using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OnSolve.Mobile.Web.ActionResults;
using OnSolve.Mobile.Web.Filters;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnSolve.Mobile.Web.Test.Filters
{
    [TestClass]
    public class ModelStateActionFilterTest
    {
        [TestMethod]
        public void Invalid_ModelState_Should_Return_ModelValidationActionResult()
        {
            
            var modelState = new ModelStateDictionary();
            modelState.AddModelError("name", "invalid");

            var actionContext = new ActionContext(
                new Mock<HttpContext>().Object,
                new Mock<RouteData>().Object,
                new Mock<ActionDescriptor>().Object,
                modelState
            );

            var actionExecutingContext = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                new Mock<ControllerBase>().Object
                );

            ModelStateActionFilter modelStateActionFilter = new ModelStateActionFilter();
            modelStateActionFilter.OnActionExecutionAsync(actionExecutingContext, Next);
            //Assert
            actionExecutingContext.Result.Should().NotBeNull()
            .And.BeOfType<ModelValidationActionResult>();
        }

        [TestMethod]
        public void Valid_ModelState_Should_NotSet_ActionResult()
        {
            var modelState = new ModelStateDictionary();            

            var actionContext = new ActionContext(
                new Mock<HttpContext>().Object,
                new Mock<RouteData>().Object,
                new Mock<ActionDescriptor>().Object,
                modelState
            );

            var actionExecutingContext = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                new Mock<ControllerBase>().Object
            );

            ModelStateActionFilter modelStateActionFilter = new ModelStateActionFilter();
            modelStateActionFilter.OnActionExecutionAsync(actionExecutingContext, Next);
            //Assert
            actionExecutingContext.Result.Should().BeNull();               
        }
        private async Task<ActionExecutedContext> Next()
        {
            return new ActionExecutedContext(
                new ActionContext(),
                new List<IFilterMetadata>(),
                new Mock<ControllerBase>().Object
                );
        }
    }
}
