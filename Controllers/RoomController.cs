using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RoomBookingApi.Data;
using RoomBookingApi.Models;
using RoomBookingApi.Services;
using RoomBookingApi.Mappers;
using RoomBookingApi.Validations;

namespace RoomBookingApi.Controllers
{

    [ApiController]
    [Route("/room")]

    // [Route("api/v{version:apiVersion}/room")]
    // [ApiVersion("1.0")]
    // [ApiVersion("2.0")]
    public class RoomController : ControllerBase
    {

        private readonly RoomApiContext _context;
        private readonly ILogger<RoomController> _logger;
        private readonly JwtTokenService _jwtTokenService;

        public RoomController(RoomApiContext context, ILogger<RoomController> logger, JwtTokenService jwtTokenService)
        {
            _context = context;
            _logger = logger;
            _jwtTokenService = jwtTokenService;
        }

        [HttpGet]
        // [MapToApiVersion("1.0")]
        public ActionResult<IEnumerable<Room>> GetRooms()
        {
            _logger.LogInformation("Get all rooms");

            var rooms = _context.Rooms;

            var roomDtos = rooms.ToDto();

            return Ok(roomDtos);
        }

        [HttpGet("{Id}")]
        public ActionResult<RoomDto> GetRoomById(int id)
        {
            _logger.LogInformation($"Get room {id}");

            var room = _context.Rooms.FirstOrDefault(room => room.Id == id);

            var roomDto = RoomExtensions.ToDto(room);

            return Ok(roomDto);
        }

        [HttpPost]
        public ActionResult<object> AddRoom([FromBody] RoomUpdate RoomAdd)
        {
            _logger.LogInformation($"Add room {RoomAdd.NewRoom.Name}");

            var newRoom = RoomAdd.NewRoom;
            var token = RoomAdd.Token;
            var pictureFile = RoomAdd.PictureFile;

            var userId = _jwtTokenService.GetUserIdFromToken(token);

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return NotFound(new { Message = "Token invalide ou utilisateur introuvable." });
            }

            var isUserAdmin = user.IsAdmin();
            if (!isUserAdmin)
            {
                return Unauthorized(new { Message = "Vous n'avez pas le droit pour ajouter une salle." });
            }

            if (!string.IsNullOrEmpty(pictureFile))
            {
                newRoom.Picture = Convert.FromBase64String(pictureFile);
            }
                        
            _context.Rooms.Add(newRoom);
            _context.SaveChanges();
            return Created(nameof(AddRoom), new { Id = newRoom.Id });
        }

        [HttpPut]
        public ActionResult UpdateRoom([FromBody] RoomUpdate RoomUpdate)
        {
            _logger.LogInformation($"Update room {RoomUpdate.NewRoom.Name}");

            var newRoom = RoomUpdate.NewRoom;
            var token = RoomUpdate.Token;

            var userId = _jwtTokenService.GetUserIdFromToken(token);

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return NotFound(new { Message = "Token invalide ou utilisateur introuvable." });
            }

            var isUserAdmin = user.IsAdmin();
            if (!isUserAdmin)
            {
                return Unauthorized(new { Message = "Vous n'avez pas le droit pour modifier une salle." });
            }

            var oldRoom = _context.Rooms.FirstOrDefault(room => room.Id == newRoom.Id);

            if (oldRoom == null) return NotFound(new { Message = $"Room with ID {newRoom.Id} not found" });

            var properties = typeof(Room).GetProperties();

            foreach (var property in properties)
            {
                if (property.Name == "Id" || !property.CanWrite) continue;

                var oldValue = property.GetValue(oldRoom);
                var newValue = property.GetValue(newRoom);

                if (!object.Equals(newValue, oldValue))
                {
                    property.SetValue(oldRoom, newValue);
                }
            }

            _context.SaveChanges();
            return Ok(new { Message = "Vaux modifications ont été enregistrées avec succès" });
        }

        [HttpDelete]
        public ActionResult DeleteRoom([FromQuery] int roomId, [FromQuery] string token)
        {
            _logger.LogInformation($"Delete room {roomId}");

            var userId = _jwtTokenService.GetUserIdFromToken(token);

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return NotFound(new { Message = "Token invalide ou utilisateur introuvable." });
            }

            var isUserAdmin = user.IsAdmin();
            if (!isUserAdmin)
            {
                return Unauthorized(new { Message = "Vous n'avez pas le droit pour supprimer une salle." });
            }

            var room = _context.Rooms.FirstOrDefault(room => room.Id == roomId);

            if (room == null) return NotFound(new { Message = $"Room with ID {roomId} not found" });

            _context.Rooms.Remove(room);
            _context.SaveChanges();
            return Accepted(new { Message = "La salle à été supprimée avec succès" });
        }


        [HttpGet("groupe")]
        public ActionResult<IEnumerable<string>> GetRoomGroupe()
        {
            return Ok(Groupes.AllowedGroupes);
        }
    }
}
