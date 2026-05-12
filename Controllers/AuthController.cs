using System;
using System.Threading.Tasks;
using Compliance_Hub.api.Data;
using Compliance_Hub.api.DTOs;
using Compliance_Hub.api.Interfaces;
using Compliance_Hub.api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Compliance_Hub.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IJwtService _jwtService;
        private readonly IAuditService _auditService;
        private readonly IConfiguration _config;

        public AuthController(
            AppDbContext context, 
            IJwtService jwtService, 
            IAuditService auditService, 
            IConfiguration config)
        {
            _context = context;
            _jwtService = jwtService;
            _auditService = auditService;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (existingUser != null)
            {
                return Conflict(new { message = "Email is already in use" });
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var newUser = new User
            {
                Name = request.Name,
                Email = request.Email,
                PasswordHash = hashedPassword,
                Role = request.Role,
                DepartmentId = request.DepartmentId,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();

            var token = _jwtService.GenerateToken(newUser);
            var expiryInMinutes = double.Parse(_config.GetSection("Jwt:ExpiryInMinutes").Value ?? "60");

            var response = new AuthResponseDTO
            {
                Token = token,
                Email = newUser.Email,
                Name = newUser.Name,
                Role = newUser.Role,
                ExpiryAt = DateTime.UtcNow.AddMinutes(expiryInMinutes)
            };

            await _auditService.LogAsync(newUser.Id, "Created", "User", newUser.Id, "User registered successfully.");

            return CreatedAtAction(nameof(Register), new { id = newUser.Id }, response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            var token = _jwtService.GenerateToken(user);
            var expiryInMinutes = double.Parse(_config.GetSection("Jwt:ExpiryInMinutes").Value ?? "60");

            var response = new AuthResponseDTO
            {
                Token = token,
                Email = user.Email,
                Name = user.Name,
                Role = user.Role,
                ExpiryAt = DateTime.UtcNow.AddMinutes(expiryInMinutes)
            };

            await _auditService.LogAsync(user.Id, "Login", "User", user.Id, "User logged in successfully.");

            return Ok(response);
        }
    }
}
