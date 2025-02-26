using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace RoomBookingApi.Validations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class HoursFromValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string timeFrom && Hours.AllowedHoursFrom.Contains(timeFrom))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("L'heure de début n'est pas valide. Veuillez choisir une heure correcte.");
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class HoursToValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string timeTo && Hours.AllowedHoursTo.Contains(timeTo))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("L'heure de fin n'est pas valide. Veuillez choisir une heure correcte.");
        }
    }
}
