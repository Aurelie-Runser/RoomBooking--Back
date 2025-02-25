using System.ComponentModel.DataAnnotations;
using RoomBookingApi.Models;

namespace RoomBookingApi.Mappers
{
    public static class RoomExtensions
    {
        public static RoomDto ToDto(this Room room)
        {
            return new RoomDto
            {
                Id = room.Id,
                Name = room.Name,
                PictureUrl = room.Picture != null ? Convert.ToBase64String(room.Picture) : null,
                Adress = room.Adress,
                AdressComplements = room.AdressComplements,
                Groupe = room.Groupe,
                Capacity = room.Capacity,
                Area = room.Area,
                IsAccessible = room.IsAccessible,
                Surface = room.Surface,
            };
        }

        public static List<RoomDto> ToDto(this IEnumerable<Room> rooms)
        {
            return rooms.Select(room => room.ToDto()).ToList();
    }
    }
}