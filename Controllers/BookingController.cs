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

            var currentDateTime = DateTime.Now;

            var bookingsAsOrganizer = _context.Bookings
                .Where(b => b.IdOrganizer == userId)
                .ToList();

            var bookingsAsGuest = _context.Bookings
                .Where(b => b.Guests.Any(g => g.IdUser == userId))
                .ToList();

            var allBookings = bookingsAsOrganizer.Concat(bookingsAsGuest).Distinct().ToList();

            foreach (var booking in allBookings)
            {
                if (booking.Statut != Status.AllowedStatus[1] && TimeOnly.TryParse(booking.TimeTo, out TimeOnly endTime))
                {
                    DateTime bookingEndTime = booking.Day.ToDateTime(endTime);

                    if (bookingEndTime < currentDateTime)
                    {
                        booking.Statut = Status.AllowedStatus[1];
                    }
                }
            }

            _logger.LogInformation("Bookings Statut updtae successfully");
            _context.SaveChanges();

            var allBookingsDto = allBookings
                .Select(b => BookingExtensions.ToDto(b, _context))
                .ToList();
            
            return Ok(allBookingsDto);
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

            var currentDate = DateOnly.FromDateTime(DateTime.Now);
            var currentTime = TimeOnly.FromDateTime(DateTime.Now);

            var timeFrom = TimeOnly.Parse(newBooking.TimeFrom);
            var timeTo = TimeOnly.Parse(newBooking.TimeTo);

            if (newBooking.Day < currentDate || (newBooking.Day == currentDate && timeFrom < currentTime) || timeFrom >= timeTo)
            {
                return BadRequest(new { Message = "Veuillez renseigner une date future." });
            }

            newBooking.IdOrganizer = userId;
            newBooking.Statut = Status.AllowedStatus[0];

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _context.Bookings.Add(newBooking);
                    _context.SaveChanges();

                    var guests = BookingAdd.Guests;
                    if (guests == null || guests.Length == 0)
                    {
                        transaction.Rollback();
                        return BadRequest(new { Message = "Veuillez inviter au moins 1 personne." });
                    }

                    AddGuests(newBooking.Id, guests);

                    var equipments = BookingAdd.Equipments;
                    if (equipments != null && equipments.Length > 0)
                    {
                        AddEquipments(newBooking.Id, equipments);
                    }

                    _context.SaveChanges();

                    transaction.Commit();

                    return Created(nameof(AddBooking), new { Id = newBooking.Id });
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Erreur lors de l'ajout de la réservation: {ex.Message}");
                    transaction.Rollback();
                    return StatusCode(500, new { Message = "Une erreur est survenue lors de la création de la réservation. Veuillez réessayer." });
                }
            }
        }

        [HttpPut]
        public ActionResult<object> UpdateBooking([FromBody] BookingUpdate BookingUpdate)
        {
            _logger.LogInformation($"Update booking {BookingUpdate.NewBooking.Id}");

            var newBooking = BookingUpdate.NewBooking;
            var token = BookingUpdate.Token;

            var userId = _jwtTokenService.GetUserIdFromToken(token) ?? 0;
            if (userId != newBooking.IdOrganizer)
            {
                return Unauthorized(new { Message = "Vous n'êtes pas l'organisateur de cette réunion, vous ne pouvez pas la modifier." });
            }

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return NotFound(new { Message = "Token invalide ou utilisateur introuvable." });
            }

            var oldBooking = _context.Bookings.FirstOrDefault(booking => booking.Id == newBooking.Id);
            if (oldBooking == null)
            {
                return NotFound(new { Message = "Cette salle n'existe pas." });
            }

            if (newBooking.Statut == Status.AllowedStatus[1])
            {
                return BadRequest(new { Message = "Cette réunion est censée être terminée, vous ne pouvez plus la modifier." });
            }

            var currentDate = DateOnly.FromDateTime(DateTime.Now);
            var currentTime = TimeOnly.FromDateTime(DateTime.Now);

            var timeFrom = TimeOnly.Parse(newBooking.TimeFrom);
            var timeTo = TimeOnly.Parse(newBooking.TimeTo);

            if (newBooking.Day < currentDate || (newBooking.Day == currentDate && timeFrom < currentTime) || timeFrom >= timeTo)
            {
                return BadRequest(new { Message = "Veuillez renseigner une date future." });
            }

            var properties = typeof(Booking).GetProperties();

            foreach (var property in properties)
            {
                if (property.Name == "Id" || property.Name == "Guests" || !property.CanWrite) continue;

                var oldValue = property.GetValue(oldBooking);
                var newValue = property.GetValue(newBooking);

                if (!object.Equals(newValue, oldValue))
                {
                    property.SetValue(oldBooking, newValue);
                }
            }

            var guests = BookingUpdate.Guests;
            if (guests == null || guests.Length == 0)
            {
                return BadRequest(new { Message = "Veuillez inviter au moins 1 personne." });
            }
            UpdateGuests(oldBooking.Id, guests);

            var equipments = BookingUpdate.Equipments;
            UpdateEquipments(oldBooking.Id, equipments);

            _context.SaveChanges();
            return Ok(new { Message = "Vaux modifications ont été enregistrées avec succès" });
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

        [HttpDelete("cancel/{id}")]
        public ActionResult CancelBooking(int id, [FromQuery] string token)
        {
            _logger.LogInformation($"Annulation de la réservation {id}");

            var userId = _jwtTokenService.GetUserIdFromToken(token);

            if (userId == null)
            {
                return BadRequest(new { Message = "Token invalide ou utilisateur introuvable." });
            }

            var booking = _context.Bookings
                .FirstOrDefault(b => b.Id == id);

            if (booking == null)
            {
                return NotFound(new { Message = "Réservation non trouvée" });
            }

            booking.Statut = Status.AllowedStatus[2];

            _context.SaveChanges();

            return Ok(new { Message = "La réservation a été annulée avec succès" });
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

        [HttpGet("export/xlsx")]
        public ActionResult ExportToExcel()
        {
            var bookings = _context.Bookings
                .Include(b => b.Guests)
                .Select(b => b.ToDto(_context))
                .ToList();

            var excelBytes = BookingExporter.ExportToExcel(bookings);
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "reservations.xlsx");
        }

        [HttpGet("available-start-hours")]
        public ActionResult<IEnumerable<string>> GetAvailableStartHours([FromQuery] int roomId, [FromQuery] string date)
        {
            _logger.LogInformation($"Vérifie les heures disponibles pour la salle {roomId} le {date}");

            if (!DateOnly.TryParse(date, out DateOnly selectedDate))
            {
                return BadRequest("Format de date invalide");
            }

            List<string> availableHours = Hours.AllowedHoursFrom.ToList();
            TimeOnly timeNow = TimeOnly.FromDateTime(DateTime.Now);

            var bookingsOfRoom = _context.Bookings
                .Where(b => b.IdRoom == roomId && b.Day == selectedDate)
                .ToList();

            var availableStartHours = availableHours
                .Where(time =>
                {
                    var parsedTime = TimeOnly.Parse(time);

                    bool isPastHour = selectedDate == DateOnly.FromDateTime(DateTime.Now) && parsedTime < timeNow;

                    bool isAvailable = !bookingsOfRoom.Any(slot =>
                        parsedTime >= TimeOnly.Parse(slot.TimeFrom) &&
                        parsedTime < TimeOnly.Parse(slot.TimeTo));

                    return !isPastHour && isAvailable;
                })
                .ToList();
                
            return Ok(availableStartHours);
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

        private void UpdateGuests(int bookingId, int[] guestIds)
        {
            var currentGuests = _context.Guests
                .Where(g => g.IdBooking == bookingId)
                .ToList();

            var guestsToRemove = currentGuests
                .Where(g => !guestIds.Contains(g.IdUser))
                .ToList();

            _context.Guests.RemoveRange(guestsToRemove);

            var guestsToAdd = guestIds
                .Where(guestId => !currentGuests.Any(g => g.IdUser == guestId))
                .Select(guestId => new Guest
                {
                    IdBooking = bookingId,
                    IdUser = guestId
                })
                .ToList();

            _context.Guests.AddRange(guestsToAdd);
        }

        private void AddEquipments(int bookingId, EquipmentBase[] equipments)
        {
            foreach (var equipment in equipments)
            {
                _context.Equipments.Add(new Equipment
                {
                    IdBooking = bookingId,
                    Materiel = equipment.Materiel,
                    Number = equipment.Number
                });
            }

            _context.SaveChanges();
        }

        private void UpdateEquipments(int bookingId, EquipmentBase[]? equipments)
        {
            var oldEquipments = _context.Equipments
                .Where(e => e.IdBooking == bookingId)
                .ToList();
            
            if(equipments == null || equipments.Length == 0)
            {
                _context.Equipments.RemoveRange(oldEquipments);
            }
            else
            {
                foreach (var newEquipment in equipments)
                {
                    var existingEquipment = oldEquipments
                        .FirstOrDefault(e => e.Materiel == newEquipment.Materiel);

                    if (existingEquipment != null)
                    {
                        existingEquipment.Number = newEquipment.Number;
                        oldEquipments.Remove(existingEquipment);
                    }
                    else
                    {
                        _context.Equipments.Add(new Equipment
                        {
                            IdBooking = bookingId,
                            Materiel = newEquipment.Materiel,
                            Number = newEquipment.Number
                        });
                    }
                }

                foreach (var equipmentToDelete in oldEquipments)
                {
                    _context.Equipments.Remove(equipmentToDelete);
                }
            }
        }
    }
}
