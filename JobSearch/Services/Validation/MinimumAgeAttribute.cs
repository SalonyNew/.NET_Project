using System;
using System.ComponentModel.DataAnnotations;

namespace Services.Validation
{
    public class MinimumAgeAttribute : ValidationAttribute
    {
        private readonly int _minimumAge;

        public MinimumAgeAttribute(int minimumAge)
        {
            _minimumAge = minimumAge;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime Dob)
            {
                var Age = DateTime.Today.Year - Dob.Year;
                if (Dob.Date > DateTime.Today.AddYears(-Age)) Age--;

                if (Age >= _minimumAge)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult($"You must be at least {_minimumAge} years old.");
                }
            }
            return new ValidationResult("Invalid birth date");
        }
    }

    
}
