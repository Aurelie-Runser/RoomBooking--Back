using System.ComponentModel.DataAnnotations;

namespace RoomBookingApi.Validations
{
    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly int _maxSizeInBytes;

        public MaxFileSizeAttribute(int maxSizeInMB)
        {
            _maxSizeInBytes = maxSizeInMB * 1024 * 1024; // Convertire Mo en octets
            ErrorMessage = $"La taille du fichier ne doit pas dépasser {maxSizeInMB} Mo.";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is byte[] fileData && fileData.Length > _maxSizeInBytes)
            {
                return new ValidationResult(ErrorMessage);
            }
            return ValidationResult.Success;
        }
    }
}
