using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Description;

namespace PatientPortal.Infrastructure.Utility
{
    // Adapted from http://analogcoder.com/2015/11/how-to-create-header-using-swashbuckle/
    public class FromHeaderAttributeOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            foreach (var httpParameterDescriptor in apiDescription.ActionDescriptor.GetParameters().Where(e => e.GetCustomAttributes<FromHeaderAttribute>().Any()))
            {
                var parameter = operation.parameters.Single(p => p.name == httpParameterDescriptor.ParameterName);
                parameter.name = httpParameterDescriptor.GetCustomAttributes<FromHeaderAttribute>().Single().HeaderName;
                parameter.@in = "header";
            }
        }
    }
}