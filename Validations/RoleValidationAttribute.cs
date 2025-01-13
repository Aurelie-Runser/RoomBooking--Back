using System.ComponentModel.DataAnnotations;

namespace RoomBookingApi.Validations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]

    public class RoleValidationAttribute : ValidationAttribute
    {
        private readonly string[] _allowedRoles = { "admin", "user" };

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is string role && _allowedRoles.Contains(role.ToLower()))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult($"Le rôle doit être l'un des suivants : {string.Join(", ", _allowedRoles)}");
        }
    }
}