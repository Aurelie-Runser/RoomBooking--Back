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
    [Route("/auth")]

    public class AuthController : ControllerBase
    {

        private readonly RoomApiContext _context;
        private readonly JwtTokenService _jwtTokenService;

        public AuthController(RoomApiContext context, JwtTokenService jwtTokenService)
        {
            _context = context;
            _jwtTokenService = jwtTokenService;
        }

        [HttpGet("{token}")]
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

        [HttpPost("register")]
        public ActionResult<User> Register(Register user)
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
        public ActionResult<User> Login([FromBody] Login loginRequest)
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

    }
}
