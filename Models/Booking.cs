using System.ComponentModel.DataAnnotations;
using RoomBookingApi.Validations;

namespace RoomBookingApi.Models
{

    public record Booking
    {

        [Required]
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; }

        public string? Description { get; set; }

        [Required]
        public int IdRoom { get; set; }

        [Required]
        public int IdOrganizer { get; set; }

        [Required]
        public DateTime DateFrom { get; set; }

        [Required]
        public DateTime DateTo { get; set; }

        [Required]
        [StatusValidation]
        public string Statut { get; set; } = "Prévus";
    }

    public record BookingUpdate
    {

        [Required]
        public required Booking NewBooking { get; set; }

        [Required]
        public required string Token { get; set; }

    }
}