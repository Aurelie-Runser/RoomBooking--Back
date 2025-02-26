using System.ComponentModel.DataAnnotations;

namespace RoomBookingApi.Models
{

    public record Login
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public required string Email { get; set; }

        [Required]
        public required string Password { get; set; }
    }

    public record Register: User
    {
    }
}   