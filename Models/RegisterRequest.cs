using System.ComponentModel.DataAnnotations;

namespace RoomBookingApi.Models
{

    public record RegisterRequest
    {
        public string Lastname { get; set; }
        public string Firstname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Company { get; set; }
        public string Job { get; set; }
    }
}