using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SkypointSocialBackend.Data;
using SkypointSocialBackend.DTOs;
using SkypointSocialBackend.Models;

namespace SkypointSocialBackend.Controllers
{
    [Route("Auth")]
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("GoogleLogin")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto request)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken);

                var email = payload.Email;
                var name = payload.Name;

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null)
                {
                    user = new User
                    {
                        Id = Guid.NewGuid(),
                        DisplayName = name,
                        Email = email
                    };

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                }

                // Generate JWT
                var token = GenerateJwtToken(user);

                return Ok(new
                {
                    token,
                    user = new
                    {
                        user.Id,
                        user.DisplayName,
                        user.Email
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest("Invalid Google token: " + ex.Message);
            }
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("name", user.DisplayName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["ExpiryMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

}
