using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace RoomBookingApi.Validations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class GroupeValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var allowedGroupes = Groupes.AllowedGroupes;

            if (value is string groupe && allowedGroupes.Any(g => string.Equals(g, groupe, StringComparison.OrdinalIgnoreCase)))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("Ce groupe n'est pas possible, merci de renseigner un groupe valide.");
        }
    }
}