using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TossErp.Identity.API.Models;

namespace TossErp.Identity.API.Services
{
    public class JwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(ApplicationUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "your-secret-key-here-make-it-long-enough");
            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim("name", user.Name),
                new Claim("last_name", user.LastName),
                new Claim("card_number", user.CardNumber),
                new Claim("card_holder", user.CardHolderName),
                new Claim("card_security_number", user.SecurityNumber),
                new Claim("card_expiration", user.Expiration),
                new Claim("address_city", user.City),
                new Claim("address_country", user.Country),
                new Claim("address_state", user.State),
                new Claim("address_street", user.Street),
                new Claim("address_zip_code", user.ZipCode)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(2),
                Issuer = _configuration["Jwt:Issuer"] ?? "TossErp",
                Audience = _configuration["Jwt:Audience"] ?? "TossErp",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
} 
