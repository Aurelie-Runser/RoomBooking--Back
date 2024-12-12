using System.ComponentModel.DataAnnotations;

namespace RoomBookingApi.Models{

    public record Room{

        [Required]
        public int Id { get; set; } 

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Slug { get; set; }

        public string Picture { get; set; }

        public string Adress { get; set; }

        public int Groupe { get; set; }

        [Range(0, 10000)]
        public int Capacity { get; set; }

        [Range(0, 1000)]
        public decimal Area { get; set; }

        public bool isAccessible { get; set; }
    }
}