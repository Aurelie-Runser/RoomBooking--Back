using System.ComponentModel.DataAnnotations;

namespace RoomBookingApi.Validations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class EquipmentValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var allowedEquipments = AvailableEquipments.AllowedEquipments;

            if (value is string equipment && allowedEquipments.Any(g => string.Equals(g, equipment, StringComparison.OrdinalIgnoreCase)))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("Cet équipement n'est pas disponible, merci de renseigner un équipement valide.");
        }
    }
}