using System.ComponentModel.DataAnnotations;

namespace RoomBookingApi.Models
{

    public record LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}