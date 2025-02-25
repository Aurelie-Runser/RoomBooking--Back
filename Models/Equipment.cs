using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RoomBookingApi.Validations;

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
        [EquipmentValidation]
        public string materiel { get; set; } = "";

        [Required]
        [Range(0, 1000)]
        public int number { get; set; }
    }
    
    public class NewEquipment
    {

        [Required]
        [EquipmentValidation]
        public string materiel { get; set; } = "";

        [Required]
        [Range(0, 1000)]
        public int number { get; set; }
    }
}