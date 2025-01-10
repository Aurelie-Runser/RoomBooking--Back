using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace RoomBookingApi.Services{
    public class JwtTokenService{
        private readonly string _secretKey;
        private readonly int _expiryDurationInHours;

        public JwtTokenService(string secretKey, int expiryDurationInHours) {
            _secretKey = secretKey;
            _expiryDurationInHours = expiryDurationInHours;
        }

        public string GenerateToken(int userId, string email, string role) {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(new[]{
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                    new Claim(ClaimTypes.Email, email),
                    new Claim(ClaimTypes.Role, role)
                }),
                Expires = DateTime.UtcNow.AddHours(_expiryDurationInHours),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public ClaimsPrincipal? ValidateToken(string token) {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);

            try {
                var validationParameters = new TokenValidationParameters {
                    ValidateIssuer = false, // À configurer si vous utilisez un Issuer spécifique
                    ValidateAudience = false, // À configurer si vous utilisez une Audience spécifique
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateLifetime = true // Vérifie si le token est expiré
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                // Vérifie que le token est bien signé avec l'algorithme attendu
                if (validatedToken is JwtSecurityToken jwtToken &&
                    jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase)) {
                    return principal; // Renvoie les revendications de l'utilisateur
                }

                return null;
            }
            catch {
                return null;
            }
        }

        public int? GetUserIdFromToken(string token) {
            var principal = ValidateToken(token);

            if (principal != null) {
                var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);

                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId)) {
                    return userId;
                }
            }

            return null;
        }
    }
}
