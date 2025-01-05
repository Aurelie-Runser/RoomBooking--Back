using System.ComponentModel.DataAnnotations;

namespace RoomBookingApi.Models{

    public record Room{

        [Required]
        
        public int Id { get; set; } 

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        public string Picture { get; set; }

        [Required]
        public string Adress { get; set; }

        public string AdressComplements { get; set; }

        public int Groupe { get; set; }

        [Required]
        [Range(0, 10000)]
        public int Capacity { get; set; }

        [Required]
        [Range(0, 1000)]
        public decimal Area{ get; set; }

        public bool IsAccessible { get; set; }

        public string Surface { get; set; } // intérieur et/ou exterieur
    }
}