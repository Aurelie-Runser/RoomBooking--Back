using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoomBookingApi.Models
{
    public class Equipment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Booking")]
        public int IdBooking { get; set; }
        public Booking Booking { get; set; } = null!;

        [Required]
        public string materiel { get; set; } = "";

        [Required]
        public int number { get; set; }
    }
}