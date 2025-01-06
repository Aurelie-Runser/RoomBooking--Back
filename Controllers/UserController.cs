using Microsoft.AspNetCore.Mvc;
using RoomBookingApi.Data;
using RoomBookingApi.Models;
using RoomBookingApi.Services;
using RoomBookingApi.Mappers;

namespace RoomBookingApi.Controllers {

    [ApiController]
    [Route("/user")]
    
    public class UserController : ControllerBase {

        private readonly RoomApiContext _context;
        private readonly JwtTokenService _jwtTokenService;

        public UserController(RoomApiContext context, JwtTokenService jwtTokenService){
            _context = context;
            _jwtTokenService = jwtTokenService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<UserBase>> GetUsers() {
            return Ok(_context.Users);
        }

        [HttpGet("{Id}")]
        public ActionResult<User> GetUserById(int id){
            return Ok(_context.Users.FirstOrDefault(user => user.Id == id));
        }

        [HttpPost]
        public ActionResult<User> AddUser(User user){
            
            if (_context.Users.Any(u => u.Email == user.Email)){
                return Conflict(new { Message = "Un utilisateur avec le même email existe déjà." });
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            _context.Users.Add(user);
            _context.SaveChanges();
            return Created(nameof(AddUser), user);
        }

        [HttpPost("login")]
        public ActionResult<User> Login([FromBody] LoginRequest loginRequest) {
            
            var user = _context.Users.FirstOrDefault(u => u.Email == loginRequest.Email);

            if (user == null) {
                return Unauthorized(new { Message = "Aucun compte avec cet email n'existe" });
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.Password);

            if (!isPasswordValid) {
                return Unauthorized(new { Message = "Ce mot de passe est mauvais pour cet identifiant." });
            }

            var token = _jwtTokenService.GenerateToken(user.Id, user.Email, user.Role.ToString());

            return Ok(new { Token = token });
        }

        [HttpPut]
        public ActionResult<User> UpdateUser(User newUser){
            var oldUser = _context.Users.FirstOrDefault(user => user.Id == newUser.Id);

            if (oldUser == null) return NotFound(new { Message = $"Aucun utilisateur avec l'ID {newUser.Id} n'a été trouvé." });

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
        public ActionResult<UserBase> DeleteUser(int id){
            var user = _context.Users.FirstOrDefault(user => user.Id == id);

            if (user == null) return NotFound(new { Message = $"Aucun utilisateur avec l'ID {id} n'a été trouvé." });

            _context.Users.Remove(user);
            _context.SaveChanges();
            return Accepted();
        }
    }
}
