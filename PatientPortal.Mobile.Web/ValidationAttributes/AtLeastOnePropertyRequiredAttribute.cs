using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace PatientPortal.Mobile.Web.ValidationAttributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class AtLeastOnePropertyRequiredAttribute : ValidationAttribute
    {
        private string[] propertyList { get; set; }

        public AtLeastOnePropertyRequiredAttribute(params string[] propertyList)
        {
            this.propertyList = propertyList;
        }

        public override object TypeId => this;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            PropertyInfo propertyInfo;
            ValidationResult validationResult;
            bool result = false;

            foreach (string propertyName in propertyList)
            {
                propertyInfo = value.GetType().GetProperty(propertyName);

                if (propertyInfo != null)
                {
                    if (propertyInfo.PropertyType == typeof(string))
                    {
                        object propertyValue = propertyInfo.GetValue(value, null);
                        result = propertyValue != null && !string.IsNullOrWhiteSpace(propertyValue.ToString());
                    }
                    else if (propertyInfo.PropertyType == typeof(int))
                    {
                        result = (int)propertyInfo.GetValue(value, null) != default(int);
                    }
                    else if (propertyInfo.PropertyType == typeof(double))
                    {
                        result = (int)propertyInfo.GetValue(value, null) != default(int);
                    }
                }

                if (result)
                {
                    break;
                }
            }

            if (result)
            {
                validationResult = ValidationResult.Success;
            }
            else
            {
                validationResult = new ValidationResult("At least one field is required", new List<string>() { string.Join(",", propertyList) });
            }
            return validationResult;
        }
    }
}
