using System.ComponentModel.DataAnnotations;

namespace RoomBookingApi.Models
{

    public record RegisterRequest
    {
        [Required]
        public required string Lastname { get; set; }

        public string? Firstname { get; set; }

        [Required]
        public required string Email { get; set; }

        [Required]
        public required string Password { get; set; }

        public string? Company { get; set; }

        public string? Job { get; set; }
    }
}   