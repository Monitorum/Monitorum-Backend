using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Monitorum.Data;
using Monitorum.Models;
using Monitorum.Services;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Monitorum.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly MonitorumDbContext _context;
        private readonly TokenService _tokenService;
        public AuthController(TokenService tokenService, MonitorumDbContext dbContext)
        {
            _tokenService = tokenService;
            _context = dbContext;
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == Int32.Parse(userId!));

            if (user == null) return BadRequest(new { message = "   User not found" });

            return Ok(new UserResponseDto
            {
                Id = user.Id,
                Username = user.Username
            });
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegistrationDto request)
        {
            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
                return BadRequest(new {message = "Username is already registered" });

            CreatePasswordHash(request.Password, out byte[] hash, out byte[] salt);

            var user = new User
            {
                Username = request.Username,
                PasswordHash = hash,
                PasswordSalt = salt,
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return Ok(new { message = "User created" });
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (user == null) return BadRequest(new { message = "User not found"});

            if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
                return BadRequest(new { message = "Wrong password"});

            var token = _tokenService.CreateToken(user);
            return Ok(new { 
                message = "Login successfull",
                token,
                user = new UserResponseDto
                {
                    Id = user.Id,
                    Username = user.Username,
                }
            });
        }

        private void CreatePasswordHash(string password, out byte[] hash, out byte[] salt)
        {
            using var hmac = new HMACSHA512();
            salt = hmac.Key;
            hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
        private bool VerifyPasswordHash(string password, byte[] hash, byte[] salt)
        {
            using var hmac = new HMACSHA512(salt);
            var computed = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computed.SequenceEqual(hash);
        }
    }    

    public class UserRegistrationDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
    public class UserLoginDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
    }
}
