using System.ComponentModel.DataAnnotations;
using RoomBookingApi.Models;
using RoomBookingApi.Data;

namespace RoomBookingApi.Mappers
{
    public static class BookingExtensions
    {
        public static BookingDto ToDto(this Booking booking, RoomApiContext _context)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == booking.IdOrganizer);
            var room = _context.Rooms.FirstOrDefault(r => r.Id == booking.IdRoom);
            
            var guestNames = booking.Guests
                .Select(g => _context.Users
                    .Where(u => u.Id == g.IdUser)
                    .Select(u => $"{u.Firstname} {u.Lastname}")
                    .FirstOrDefault())
                .ToList()
                .ToArray();

            return new BookingDto
            {
                Id = booking.Id,
                Name = booking.Name,
                IdOrganizer = booking.IdOrganizer,
                OrganizerLastname = user?.Lastname ?? "Inconnu",
                OrganizerFirstname = user?.Firstname ?? "Inconnu",
                IdRoom = booking.IdRoom,
                RoomName = room?.Name ?? "Inconnu",
                DateFrom = booking.DateFrom,
                DateFormat = booking.DateFrom.ToString("dd/MM/yyyy"),
                TimeFromFormat = booking.DateFrom.ToString("HH:mm"),
                TimeToFormat = booking.DateTo.ToString("HH:mm"),
                Statut = booking.Statut,
                GuestsName = guestNames!,
            };
        }
    }
}
