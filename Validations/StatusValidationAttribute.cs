using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace RoomBookingApi.Validations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class StatusValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var allowedStatus = Status.AllowedStatus;

            if (value is string status && allowedStatus.Any(g => string.Equals(g, status, StringComparison.OrdinalIgnoreCase)))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("Ce statut n'est pas possible, merci de renseigner un statut valide.");
        }
    }
}