using System.ComponentModel.DataAnnotations;

namespace RoomBookingApi.Validations {
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]

    public class GroupeValidationAttribute : ValidationAttribute {
        private readonly string[] _allowedGroupes = { "", "Petite Réunion",  "Moyenne Réunion",  "Grande Réunion", "Conférence", "Fête", "Gala" };

        protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
            if (value is string role && _allowedGroupes.Contains(role.ToLower())) {
                return ValidationResult.Success;
            }

            return new ValidationResult($"Le groupe doit être l'un des suivants : {string.Join(", ", _allowedGroupes)}");
        }
    }
}