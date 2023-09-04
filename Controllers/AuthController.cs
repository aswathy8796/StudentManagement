using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StudentManagementApi.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StudentManagementApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private IConfiguration _config;
        private static readonly Dictionary<string, LoginUser> _users = new Dictionary<string, LoginUser>
        {
            { "admin", new LoginUser { Username = "admin", Password = "password123" } }
        };

        public AuthController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login(LoginUser login)
        {
            if (_users.TryGetValue(login.Username, out var user) && login.Password == user.Password)
            {
                // Generate and return a JWT token
                var token = GenerateJwtToken(login.Username);

                // Return tokens to the client
                return Ok(new { token });
            }
            return Unauthorized("Invalid credentials. Enter valid username and password.");
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public IActionResult Refresh()
        {
            var httpContext = HttpContext;
            var headers = httpContext.Request.Headers;
            if (headers.TryGetValue("Authorization", out var authHeader))
            {
                var token = authHeader.ToString().Replace("Bearer ", string.Empty);
                var tokenHandler = new JwtSecurityTokenHandler();
                var claims = tokenHandler.ReadJwtToken(token).Claims;
                var usernameClaim = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);
                if (usernameClaim != null)
                {
                    var username = usernameClaim.Value;
                    var newAccessToken = GenerateJwtToken(username);
                    return Ok(new { newToken = newAccessToken });
                }
            }
            return Unauthorized();

        }

        private string GenerateJwtToken(string username)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "teacher"), // Sample role 
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        
    }
}


 