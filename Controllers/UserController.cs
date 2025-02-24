using Microsoft.AspNetCore.Mvc;
using RoomBookingApi.Data;
using RoomBookingApi.Models;
using RoomBookingApi.Services;
using RoomBookingApi.Mappers;
using RoomBookingApi.Validations;
using System.Reflection;

namespace RoomBookingApi.Controllers
{

    [ApiController]
    [Route("/user")]

    public class UserController : ControllerBase
    {

        private readonly RoomApiContext _context;
        private readonly JwtTokenService _jwtTokenService;

        public UserController(RoomApiContext context, JwtTokenService jwtTokenService)
        {
            _context = context;
            _jwtTokenService = jwtTokenService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<UserBase>> GetUsers()
        {
            var users = _context.Users
                .Select(u => new UserBase
                {
                    Id = u.Id,
                    Lastname = u.Lastname,
                    Firstname = u.Firstname
                })
                .ToList();

            return Ok(users);
        }

        [HttpGet("{Id}")]
        public ActionResult<User> GetUserById(int id)
        {
            return Ok(_context.Users.FirstOrDefault(user => user.Id == id));
        }

        [HttpGet("profil/{token}")]
        public ActionResult<User> getProfil(string token)
        {
            var userId = _jwtTokenService.GetUserIdFromToken(token);

            if (userId == null)
            {
                return BadRequest(new { Message = "Token invalide ou utilisateur introuvable." });
            }

            var user = _context.Users.FirstOrDefault(user => user.Id == userId);

            if (user == null)
            {
                return NotFound(new { Message = $"Aucun utilisateur avec l'ID {userId} n'a été trouvé." });
            }

            var userDto = UserExtensions.ToDto(user);

            return Ok(userDto);
        }

        [HttpPost]
        public ActionResult<User> Register(User user)
        {

            if (_context.Users.Any(u => u.Email == user.Email))
            {
                return Conflict(new { Message = "Un utilisateur avec le même email existe déjà. Connectez-vous ou essayer avec une autre adresse mail" });
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            _context.Users.Add(user);
            _context.SaveChanges();

            var token = _jwtTokenService.GenerateToken(user.Id, user.Email);

            return Ok(new { Token = token, IsAdmin = false, Message = "Votre compte à bien été créé." });
        }

        [HttpPost("login")]
        public ActionResult<User> Login([FromBody] LoginRequest loginRequest)
        {

            var user = _context.Users.FirstOrDefault(u => u.Email == loginRequest.Email);

            if (user == null)
            {
                return Unauthorized(new { Message = "Aucun compte avec cet email n'existe" });
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.Password);

            if (!isPasswordValid)
            {
                return Unauthorized(new { Message = "Ce mot de passe est mauvais pour cet identifiant." });
            }

            var token = _jwtTokenService.GenerateToken(user.Id, user.Email);
            var isUserAdmin = user.IsAdmin();

            return Ok(new { Token = token, IsAdmin = isUserAdmin });
        }

        [HttpPut]
        public ActionResult<User> UpdateUser(User newUser)
        {
            var oldUser = _context.Users.FirstOrDefault(user => user.Id == newUser.Id);

            if (oldUser == null) return NotFound(new { Message = $"Aucun utilisateur avec l'ID {newUser.Id} n'a été trouvé." });

            var properties = typeof(User).GetProperties();

            foreach (var property in properties)
            {
                var oldValue = property.GetValue(oldUser);
                var newValue = property.GetValue(newUser);

                if (!object.Equals(newValue, oldValue))
                {
                    if (property.Name == "Id" || property.Name == "Role" || property.Name == "Email")
                    {
                        return Unauthorized(new { Message = "Vous n'avez pas les droits de modificer votre Id, votre adresse mail ou votre rôle." });
                    };

                    if (property.Name == "Password")
                    {

                        if (string.IsNullOrWhiteSpace(newValue?.ToString())) continue;

                        newValue = BCrypt.Net.BCrypt.HashPassword(newValue.ToString());
                    }

                    property.SetValue(oldUser, newValue);
                }
            }

            _context.SaveChanges();
            return Ok(new { Message = "Vaux modifications ont été enregistrées avec succès" });
        }

        [HttpDelete("{Id}")]
        public ActionResult<UserBase> DeleteUser(int id)
        {
            var user = _context.Users.FirstOrDefault(user => user.Id == id);

            if (user == null) return NotFound(new { Message = $"Aucun utilisateur avec l'ID {id} n'a été trouvé." });

            _context.Users.Remove(user);
            _context.SaveChanges();
            return Accepted(new { Message = "Votre compte à été supprimé avec succès" });
        }

        
        [HttpGet("admin/{token}")]
        public ActionResult<IEnumerable<UserBase>> GetUsersAdmin(string token)
        {
            var userId = _jwtTokenService.GetUserIdFromToken(token);

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return NotFound(new { Message = "Token invalide ou utilisateur introuvable." });
            }

            var isUserAdmin = user.IsAdmin();
            if (!isUserAdmin)
            {
                return Unauthorized(new { Message = "Vous n'avez pas les droits administrateurs pour voir la liste des utilisateurs." });
            }

            var users = _context.Users
                .Select(u => new UserBase
                {
                    Id = u.Id,
                    Lastname = u.Lastname,
                    Firstname = u.Firstname,
                    Role = u.Role
                })
                .ToList();

            return Ok(users);
        }

        [HttpPut("admin/{token}")]
        public ActionResult<IEnumerable<UserBase>> UpdateUsersAdmin([FromBody] UserForAdmin[] listUser, [FromRoute] string token)
        {
            var userId = _jwtTokenService.GetUserIdFromToken(token);

            var userConnect = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (userConnect == null) return NotFound(new { Message = "Token invalide ou utilisateur introuvable." });

            var isUserAdmin = userConnect.IsAdmin();
            if (!isUserAdmin) return Unauthorized(new { Message = "Vous n'avez pas les droits administrateurs pour modifier les rôles des utilisateurs." });
            
            var userIds = listUser.Select(u => u.Id).ToList();
            var usersInDb = _context.Users.Where(u => userIds.Contains(u.Id)).ToList();

            if (usersInDb.Count == 0) return NotFound(new { Message = "Aucun des utilisateurs fournis n'a été trouvé." });

            foreach (var user in listUser)
            {
                var oldUser = usersInDb.FirstOrDefault(u => u.Id == user.Id);
                if (oldUser == null) return NotFound(new { Message = $"Aucun utilisateur avec l'ID {user.Id} n'a été trouvé." });

                if (!object.Equals(oldUser.Role, user.Role))
                {
                    if(user.Id == userConnect.Id) return Unauthorized(new { Message = "Vous ne pouvez pas modifier votre propre rôle." });

                    oldUser.Role = user.Role;
                }
            }

            _context.SaveChanges();
            return Ok(new { Message = "Vaux modifications ont été enregistrées avec succès." });
        }
    }
}
