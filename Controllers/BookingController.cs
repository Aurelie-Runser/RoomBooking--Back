using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RoomBookingApi.Data;
using RoomBookingApi.Models;
using RoomBookingApi.Services;
using RoomBookingApi.Mappers;
using RoomBookingApi.Validations;
using Microsoft.EntityFrameworkCore;

namespace RoomBookingApi.Controllers
{

    [ApiController]
    [Route("/booking")]

    public class BookingController : ControllerBase
    {

        private readonly RoomApiContext _context;
        private readonly ILogger<BookingController> _logger;
        private readonly JwtTokenService _jwtTokenService;

        public BookingController(RoomApiContext context, ILogger<BookingController> logger, JwtTokenService jwtTokenService)
        {
            _context = context;
            _logger = logger;
            _jwtTokenService = jwtTokenService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Booking>> GetBookings()
        {
            _logger.LogInformation("Get all booking");
            return Ok(_context.Bookings);
        }

        [HttpGet("user/{token}")]
        public ActionResult<IEnumerable<BookingDto>> GetBookingsUser(string token)
        {
            _logger.LogInformation("Get booking of a user");

            var userId = _jwtTokenService.GetUserIdFromToken(token);

            if (userId == null)
            {
                return BadRequest(new { Message = "Token invalide ou utilisateur introuvable." });
            }

            var bookingsAsOrganizer = _context.Bookings
                .Where(b => b.IdOrganizer == userId)
                .Select(b => BookingExtensions.ToDto(b, _context))
                .ToList();

            var bookingsAsGuest = _context.Bookings
                .Where(b => b.Guests.Any(g => g.IdUser == userId))
                .Select(b => BookingExtensions.ToDto(b, _context))
                .ToList();

            var allBookings = bookingsAsOrganizer
                .Concat(bookingsAsGuest)
                .Distinct()
                .ToList();
            
            return Ok(allBookings);
        }

        [HttpGet("{Id}")]
        public ActionResult<BookingDto> GetBookingById(int id)
        {
            _logger.LogInformation($"Get boooking {id}");

           var booking = _context.Bookings
                            .Include(b => b.Guests)
                            .FirstOrDefault(b => b.Id == id);

            if (booking == null)
            {
                return NotFound(new { Message = "Réservation non trouvée" });
            }

            var bookingDto = BookingExtensions.ToDto(booking, _context);

            return Ok(bookingDto);
        }

        [HttpPost]
        public ActionResult<object> AddBooking([FromBody] BookingUpdate BookingAdd)
        {
            _logger.LogInformation($"Add booking");

            var newBooking = BookingAdd.NewBooking;
            var token = BookingAdd.Token;

            var userId = _jwtTokenService.GetUserIdFromToken(token) ?? 0;

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return NotFound(new { Message = "Token invalide ou utilisateur introuvable." });
            }

            var room = _context.Rooms.FirstOrDefault(room => room.Id == newBooking.IdRoom);
            if (room == null)
            {
                return NotFound(new { Message = "Cette salle n'existe pas." });
            }

            var TimeFrom = TimeOnly.Parse(newBooking.TimeFrom);
            var TimeTo = TimeOnly.Parse(newBooking.TimeTo);

            if ((newBooking.Day < DateOnly.FromDateTime(DateTime.Now) && TimeFrom < TimeOnly.FromDateTime(DateTime.Now)) || TimeFrom > TimeTo)
            {
                return BadRequest(new { Message = "Veuillez renseigner une date future." });
            }

            newBooking.IdOrganizer = userId;
            newBooking.Statut = Status.AllowedStatus[0];

            _context.Bookings.Add(newBooking);
            _context.SaveChanges();

            var guests = BookingAdd.Guests;

            if (guests != null && guests.Length > 0)
            {
                AddGuests(newBooking.Id, guests);
            }

            _context.SaveChanges();
            return Created(nameof(AddBooking), new { Id = newBooking.Id });
        }

        [HttpGet("room/{roomId}")]
        public ActionResult<IEnumerable<BookingDto>> GetBookingsByRoom(int roomId)
        {
            _logger.LogInformation($"Get bookings for room {roomId}");

            var room = _context.Rooms.FirstOrDefault(r => r.Id == roomId);

            if (room == null)
            {
                _logger.LogWarning($"Room {roomId} not found");
                return NotFound(new { Message = "Salle non trouvée" });
            }

            _logger.LogInformation($"Room found: {room.Id}");

            var bookings = _context.Bookings
                .Include(b => b.Guests)
                .Where(b => b.IdRoom == roomId)
                .ToList();
                
            _logger.LogInformation($"Raw bookings count: {bookings.Count}");

            var bookingDtos = bookings
                .Select(b => BookingExtensions.ToDto(b, _context))
                .ToList();

            _logger.LogInformation($"DTO bookings count: {bookingDtos.Count}");

            return Ok(bookingDtos);
        }

        [HttpGet("available-start-hours")]
        public ActionResult<IEnumerable<string>> GetAvailableStartHours([FromQuery] int roomId, [FromQuery] string date)
        {
            _logger.LogInformation($"Vérifie les heures disponibles pour la salle {roomId} le {date}");

            if (!DateOnly.TryParse(date, out DateOnly selectedDate))
            {
                return BadRequest("Format de date invalide");
            }

            List<string> availableHours = GenerateAvailableHours();

            var bookedSlots = _context.Bookings
                .Where(b => b.IdRoom == roomId && b.Day == selectedDate)
                .ToList();

            var availableStartHours = availableHours
                .Where(time =>
                {
                    var parsedTime = TimeOnly.Parse(time);
                    return !bookedSlots.Any(slot => parsedTime >= TimeOnly.Parse(slot.TimeFrom) && parsedTime < TimeOnly.Parse(slot.TimeTo));
                })
                .ToList();
                
            return Ok(availableStartHours);
        }

        private List<string> GenerateAvailableHours()
        {
            var availableHours = new List<string>();
            for (int hour = 7; hour <= 23; hour++)
            {
                for (int minutes = 0; minutes < 60; minutes += 15)
                {
                    availableHours.Add($"{hour:D2}:{minutes:D2}");
                }
            }
            return availableHours;
        }


        private void AddGuests(int bookingId, int[] guestIds)
        {
            foreach (var guestId in guestIds)
            {
                _context.Guests.Add(new Guest
                {
                    IdBooking = bookingId,
                    IdUser = guestId
                });
            }

            _context.SaveChanges();
        }

    }
}
