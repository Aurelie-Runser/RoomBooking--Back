using System.ComponentModel.DataAnnotations;
using RoomBookingApi.Validations;

namespace RoomBookingApi.Models
{

    public record Booking
    {

        [Required]
        public int Id { get; set; }

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
        public DateTime Statut { get; set; }
    }
}