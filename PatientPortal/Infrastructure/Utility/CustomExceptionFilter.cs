using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PatientPortal.Infrastructure.Utility
{
    public class CustomExceptionFilter : ActionFilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            Exception e = filterContext.Exception;
            filterContext.ExceptionHandled = true;
            filterContext.Result = new ViewResult()
            {
                ViewName = "ExceptionPage"
            };
        }
    }

    //public interface IExceptionFilter
    //{
    //    void OnException(ExceptionContext filterContext);
    //}
}