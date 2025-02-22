using System.ComponentModel.DataAnnotations;
using RoomBookingApi.Validations;

namespace RoomBookingApi.Models
{

    public record Room
    {

        [Required]

        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        public byte[]? Picture { get; set; }

        [Required]
        public string Adress { get; set; } = string.Empty;

        public string AdressComplements { get; set; } = string.Empty;

        [GroupeValidation]
        public string Groupe { get; set; } = "";

        [Required]
        [Range(0, 10000)]
        public int Capacity { get; set; }

        [Required]
        [Range(0, 1000)]
        public decimal Area { get; set; }

        public bool IsAccessible { get; set; }

        public string Surface { get; set; } = string.Empty; // intérieur et/ou exterieur
    }

    public record RoomDto : Room
    {
        public required string PictureUrl { get; set; }
    }

    public record RoomUpdate
    {

        [Required]
        public required Room NewRoom { get; set; }

        [Required]
        public required string Token { get; set; }

        public string? PictureFile { get; set; }

    }

    public record RoomDelete
    {

        [Required]
        public int roomId { get; set; }

        [Required]
        public required string Token { get; set; }

    }
}