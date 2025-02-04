using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

        public List<Guest> Guests { get; set; } = new();
    }

    public record BookingDto : Booking
    {
        public string? RoomName { get; set; }
        public string? OrganizerFirstname { get; set; }
        public string? OrganizerLastname { get; set; }
        public string? DateFormat { get; set; }
        public string? TimeFromFormat { get; set; }
        public string? TimeToFormat { get; set; }
    }

    public record BookingUpdate
    {

        [Required]
        public required Booking NewBooking { get; set; }

        [Required]
        public required string Token { get; set; }

        public int[]? Guests { get; set; }
    }
}