using System.ComponentModel.DataAnnotations;

namespace RoomBookingApi.Models
{

    public record Login
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public required string Email { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 4, ErrorMessage = "Le mot de passe doit contenir entre 4 et 20 caractères.")]
        public required string Password { get; set; }
    }

    public record Register: User
    {
    }
}   