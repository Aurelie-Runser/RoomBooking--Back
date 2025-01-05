using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RoomBookingApi.Models;
using RoomBookingApi.Data;
using BCrypt.Net;

namespace RoomBookingApi.Controllers {

    [ApiController]
    [Route("/user")]
    
    public class UserController : ControllerBase {

        private readonly RoomApiContext _context;

        public UserController(RoomApiContext context){
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<User>> GetUsers() {
            return Ok(_context.Users);
        }

        [HttpGet("{Id}")]
        public ActionResult<User> GetUserById(int id){
            return Ok(_context.Users.FirstOrDefault(user => user.Id == id));
        }

        [HttpPost]
        public ActionResult<User> AddUser(User user){
            
            if (_context.Users.Any(u => u.Email == user.Email)){
                return Conflict(new { Message = "An user with the same email already exists." });
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            _context.Users.Add(user);
            _context.SaveChanges();
            return Created(nameof(AddUser), user);
        }

        [HttpPut]
        public ActionResult<User> UpdateUser(User newUser){
            var oldUser = _context.Users.FirstOrDefault(user => user.Id == newUser.Id);

            if (oldUser == null) return NotFound(new { Message = $"User with ID {newUser.Id} not found" });

            var properties = typeof(User).GetProperties();

            foreach (var property in properties){
                if (property.Name == "Id" || !property.CanWrite) continue;

                var oldValue = property.GetValue(oldUser);
                var newValue = property.GetValue(newUser);

                if (!object.Equals(newValue, oldValue)){

                    if(property.Name == "Password"){
                        newValue = BCrypt.Net.BCrypt.HashPassword(newValue.ToString());
                    }
                    
                    property.SetValue(oldUser, newValue);
                }
            }

            _context.SaveChanges();
            return Accepted(newUser);
        }

        [HttpDelete]
        public ActionResult<User> DeleteUser(int id){
            var user = _context.Users.FirstOrDefault(user => user.Id == id);

            if (user == null) return NotFound(new { Message = $"User with ID {id} not found" });

            _context.Users.Remove(user);
            _context.SaveChanges();
            return Accepted();
        }
    }
}
