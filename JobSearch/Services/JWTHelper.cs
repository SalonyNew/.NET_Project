
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Services
{
    public class JWTHelper
    {
        private readonly IConfiguration _configuration;

        public JWTHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GenerateJwtToken(string userEmail, string userRole)
        {
            var jwtkey = _configuration["jwtSecretKey"];
            var key = Encoding.ASCII.GetBytes(_configuration["jwtSecretKey"]!);
            var securityKey = new SymmetricSecurityKey(key);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);


            var Loginclaims = new[]
            {
                new Claim(ClaimTypes.Email,userEmail),
                new Claim(ClaimTypes.Role,userRole)
    };


            var token = new JwtSecurityToken(
                claims: Loginclaims,
                issuer: _configuration["jwtissuer"],
                audience: _configuration["jwtaudience"],
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials
            );



            return new JwtSecurityTokenHandler().WriteToken(token);

            

        }
    }
}
