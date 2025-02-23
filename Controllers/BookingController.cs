using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RoomBookingApi.Data;
using RoomBookingApi.Models;
using RoomBookingApi.Services;
using RoomBookingApi.Mappers;
using RoomBookingApi.Validations;
using Microsoft.EntityFrameworkCore;
using System.Text;
using RoomBookingApi.Utils;

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

            newBooking.IdOrganizer = userId;
            newBooking.Statut = Status.AllowedStatus[0];

            _context.Bookings.Add(newBooking);
            _context.SaveChanges();

            var guests = BookingAdd.Guests;

            if (guests != null && guests.Length > 0)
            {
                foreach(var guest in guests)
                {
                    _context.Guests.Add(new Guest 
                    {
                        IdBooking = newBooking.Id,
                        IdUser = guest
                    });
                }
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

        [HttpGet("export/csv")]
        public ActionResult ExportToCsv()
        {
            var bookings = _context.Bookings
                .Include(b => b.Guests)
                .Select(b => b.ToDto(_context))
                .ToList();

            var csvContent = BookingExporter.ExportToCsv(bookings);
            var bytes = Encoding.UTF8.GetBytes(csvContent);

            return File(bytes, "text/csv", "reservations.csv");
        }

        [HttpGet("export/ical")]
        public ActionResult ExportToICalendar()
        {
            var bookings = _context.Bookings
                .Include(b => b.Guests)
                .Select(b => b.ToDto(_context))
                .ToList();

            var icalContent = BookingExporter.ExportToCalendar(bookings);
            var bytes = Encoding.UTF8.GetBytes(icalContent);

            return File(bytes, "text/calendar", "reservations.ics");
        }

        [HttpGet("export/cal")]
        public ActionResult ExportToCalendar()
        {
            var bookings = _context.Bookings
                .Include(b => b.Guests)
                .Select(b => b.ToDto(_context))
                .ToList();

            var calContent = BookingExporter.ExportToCalendar(bookings);
            var bytes = Encoding.UTF8.GetBytes(calContent);

            return File(bytes, "text/calendar", "reservations.cal");
        }
    }
}
