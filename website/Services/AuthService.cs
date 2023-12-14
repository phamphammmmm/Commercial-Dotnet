using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using website.Models;
using website.Services;

namespace website.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;

        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<AuthenticationResult> Authenticate(User user)
        {
            if (user == null || string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password))
            {
                Console.WriteLine("Invalid user information");
                return new AuthenticationResult { Success = false, Token = null };
            }

            Console.WriteLine($"Received user info: {user.Name}, {user.Email}, {user.Role}");

            // Payload
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
            };


            // Signature
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(int.Parse(_configuration["Jwt:ExpirationInMinutes"])),
                signingCredentials: creds
            );

           // Console.WriteLine($"Token expiration: {token.ValidTo}");

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            // Log token để kiểm tra
            //Console.WriteLine("Token created: " + tokenString);

            // Create and return AuthenticationResult
            return new AuthenticationResult
            {
                Success = true,
                Token = tokenString
            };
        }
    }
}
