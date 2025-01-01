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

            if( oldRoom == null) return NotFound(newRoom.Id);

            oldRoom.Name = newRoom.Name;
            oldRoom.Slug = newRoom.Slug;
            oldRoom.Picture = newRoom.Picture;
            oldRoom.Adress = newRoom.Adress;
            oldRoom.Groupe = newRoom.Groupe;
            oldRoom.Capacity = newRoom.Capacity;
            oldRoom.Area = newRoom.Area;
            oldRoom.IsAccessible = newRoom.IsAccessible;
            oldRoom.Surface = newRoom.Surface;

            _context.SaveChanges();
            return Accepted(newRoom);
        }

        [HttpDelete]
        public ActionResult<Room> DeleteRoom(int id){
            var room = _context.Rooms.FirstOrDefault(room => room.Id == id);

            if( room == null) return NotFound(id);

            _context.Rooms.Remove(room);
            _context.SaveChanges();
            return Accepted();
        }
    }
}
