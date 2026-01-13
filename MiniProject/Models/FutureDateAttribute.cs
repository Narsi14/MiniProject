using System;
using System.ComponentModel.DataAnnotations;

namespace MiniProject.Models
{
    public class FutureDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is DateTime dateTime)
            {
                return dateTime > DateTime.Now;
            }
            return true;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} cannot be a past date.";
        }
    }
}
