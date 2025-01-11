using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RoomBookingApi.Data;
using RoomBookingApi.Models;
using RoomBookingApi.Services;
using RoomBookingApi.Validations;

namespace RoomBookingApi.Controllers {

    [ApiController]
    [Route("/room")]
    
    public class RoomController : ControllerBase {

        private readonly RoomApiContext _context;
        private readonly JwtTokenService _jwtTokenService;

        public RoomController(RoomApiContext context, JwtTokenService jwtTokenService){
            _context = context;
            _jwtTokenService = jwtTokenService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Room>> GetRooms() {
            return Ok(_context.Rooms);
        }

        [HttpGet("{Id}")]
        public ActionResult<Room> GetRoomById(int id){
            return Ok(_context.Rooms.FirstOrDefault(room => room.Id == id));
        }

        [HttpPost]
        public ActionResult<Room> AddRoom(Room room){
            _context.Rooms.Add(room);
            _context.SaveChanges();
            return Created(nameof(AddRoom), room);
        }

        [HttpPut]
        public ActionResult UpdateRoom([FromBody] RoomUpdate RoomUpdate){

            var newRoom = RoomUpdate.newRoom;
            var token = RoomUpdate.token;

            var userRole = _jwtTokenService.GetUserRoleFromToken(token);

            if (userRole == null) {
                return BadRequest(new { Message = "Token invalide ou utilisateur introuvable." });
            }

            if (userRole != "admin") {
                return Unauthorized(new { Message = "Vous n'avez pas le droit pour modifier une salle." });
            }
            
            var oldRoom = _context.Rooms.FirstOrDefault(room => room.Id == newRoom.Id);
            
            if (oldRoom == null) return NotFound(new { Message = $"Room with ID {newRoom.Id} not found" });

            var properties = typeof(Room).GetProperties();

            foreach (var property in properties){
                if (property.Name == "Id" || !property.CanWrite) continue;

                var oldValue = property.GetValue(oldRoom);
                var newValue = property.GetValue(newRoom);

                if (!object.Equals(newValue, oldValue)){
                    property.SetValue(oldRoom, newValue);
                }
            }

            _context.SaveChanges();
            return Ok(new { Message = "Vaux modifications ont été enregistrées avec succès"});
        }

        [HttpDelete]
        public ActionResult<Room> DeleteRoom(int id){
            var room = _context.Rooms.FirstOrDefault(room => room.Id == id);

            if (room == null) return NotFound(new { Message = $"Room with ID {id} not found" });

            _context.Rooms.Remove(room);
            _context.SaveChanges();
            return Accepted();
        }


        [HttpGet("groupe")]
        public ActionResult<IEnumerable<string>> GetRoomGroupe(){
            return Ok(Groupes.AllowedGroupes);
        }
    }
}
