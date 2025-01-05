using System.ComponentModel.DataAnnotations;

namespace RoomBookingApi.Models{

    public record User{

        [Required]
        public int Id { get; set; } 

        [Required]
        public string Lastname { get; set; } = string.Empty;

        public string Firstname { get; set; } = string.Empty;

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public string Company { get; set; } = string.Empty;

        public string Job { get; set; } = string.Empty;

        [Required]
        public UserRole Role { get; set; }
    }

    public enum UserRole {
        admin,
        user
    }
}