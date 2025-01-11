using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RoomBookingApi.Models;
using RoomBookingApi.Data;

namespace RoomBookingApi.Controllers {

    [ApiController]
    [Route("/room")]
    
    public class RoomController : ControllerBase {

        private readonly RoomApiContext _context;

        public RoomController(RoomApiContext context){
            _context = context;
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
        public ActionResult<Room> UpdateRoom(Room newRoom){
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
    }
}
