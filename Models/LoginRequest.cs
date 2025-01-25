using System.ComponentModel.DataAnnotations;

namespace RoomBookingApi.Models
{

    public record LoginRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}