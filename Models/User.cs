using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RoomBookingApi.Validations;

namespace RoomBookingApi.Models
{

    public record UserBase
    {

        [Required]
        public int Id { get; set; }

        [Required]
        public string Lastname { get; set; } = string.Empty;

        public string Firstname { get; set; } = string.Empty;

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        public string Company { get; set; } = string.Empty;

        public string Job { get; set; } = string.Empty;

        [Required]
        [RoleValidation]
        public string Role { get; set; } = "user";

        public List<Guest> Guests { get; set; } = new();

        public bool IsAdmin()
        {
            return string.Equals(Role, "admin", StringComparison.OrdinalIgnoreCase);
        }
    }

    public record User : UserBase
    {
        public string Password { get; set; } = string.Empty;
    }

    public record UserDto : UserBase
    {
    }

    public record UserForAdmin
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Lastname { get; set; } = string.Empty;

        public string Firstname { get; set; } = string.Empty;

        [Required]
        [RoleValidation]
        public required string Role { get; set; }
    }
}