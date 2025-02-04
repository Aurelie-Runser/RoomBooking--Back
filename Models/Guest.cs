using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoomBookingApi.Models
{
    public class Guest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Booking")]
        public int IdBooking { get; set; }
        public Booking Booking { get; set; } = null!;

        [Required]
        [ForeignKey("User")]
        public int IdUser { get; set; }
        public User User { get; set; } = null!;
    }
}