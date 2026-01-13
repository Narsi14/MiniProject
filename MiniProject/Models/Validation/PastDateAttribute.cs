using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MiniProject.Models.Validation
{
    public class PastDateAttribute : ValidationAttribute, IClientModelValidator
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;

            if (value is DateTime dt)
            {
                // Allow dates strictly before today
                if (dt.Date < DateTime.Now.Date) return ValidationResult.Success;
                return new ValidationResult(ErrorMessage ?? "Date must be in the past.");
            }

            return new ValidationResult("Invalid date value.");
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-pastdate", ErrorMessage ?? "Date must be in the past.");
        }

        private static bool MergeAttribute(IDictionary<string, string> attributes, string key, string value)
        {
            if (attributes.ContainsKey(key)) return false;
            attributes.Add(key, value);
            return true;
        }
    }
}