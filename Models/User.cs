using System.ComponentModel.DataAnnotations;

namespace RoomBookingApi.Models{

    public record User{

        [Required]
        public int Id { get; private set; } 

        [Required]
        public string Lastname { get; set; } = string.Empty;

        public string Firstname { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string Company { get; set; }

        public string Job { get; set; }

        [Required]
        public string Type { get; set; }    // admin, user...
    }
}