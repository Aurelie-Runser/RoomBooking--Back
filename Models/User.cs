using System.ComponentModel.DataAnnotations;
using RoomBookingApi.Validations;

namespace RoomBookingApi.Models
{
    public record UserMinimal
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Lastname { get; set; } = string.Empty;

        public string Firstname { get; set; } = string.Empty;

        [Required]
        [RoleValidation]
        public string Role { get; set; } = "user";
    }

    public record UserBase : UserMinimal
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        public string Company { get; set; } = string.Empty;

        public string Job { get; set; } = string.Empty;
    }

    public record User : UserBase
    {
        public string Password { get; set; } = string.Empty;
        public List<Guest> Guests { get; set; } = new();

        public bool IsAdmin()
        {
            return string.Equals(Role, "admin", StringComparison.OrdinalIgnoreCase);
        }
    }

    public record UserDto : UserBase { }
}
