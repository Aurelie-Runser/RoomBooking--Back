using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RoomBookingApi.Validations;

namespace RoomBookingApi.Models
{
    public class EquipmentBase
    {

        [Required]
        [EquipmentValidation]
        public string Materiel { get; set; } = "";

        [Required]
        [Range(0, 1000)]
        public int Number { get; set; }
    }

    public class Equipment : EquipmentBase
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Booking")]
        public int IdBooking { get; set; }
        public Booking Booking { get; set; } = null!;
    }
}