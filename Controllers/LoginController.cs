using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("UserLogin")]
        public IActionResult UserLogin([FromBody] LoginRequest loginRequest)
        {
            if (IsValidUser(loginRequest.Username, loginRequest.Password))
            {
                var token = GenerateJwtToken("User");
                return Ok(new { Token = token });
            }
            return Unauthorized();
        }

        [HttpPost("AdminLogin")]
        public IActionResult AdminLogin([FromBody] LoginRequest loginRequest)
        {
            if (IsValidAdmin(loginRequest.Username, loginRequest.Password))
            {
                var token = GenerateJwtToken("Admin");
                return Ok(new { Token = token });
            }
            return Unauthorized();
        }

        private bool IsValidUser(string username, string password)
        {
            return (username == "user" && password == "user123");
        }

        private bool IsValidAdmin(string username, string password)
        {
            return (username == "admin" && password == "admin123");
        }

        private string GenerateJwtToken(string role)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Role, role)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
