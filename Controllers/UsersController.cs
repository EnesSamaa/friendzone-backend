using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using friendzone_backend.Data;
using friendzone_backend.Entities;
using friendzone_backend.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace friendzone_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtService _jwtService;


        public UserController(AppDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        // =========================
        // REGISTER
        // =========================
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDto request)
        {
            var usernameExists = await _context.Users
                .AnyAsync(x => x.Username == request.Username);

            if (usernameExists)
                return BadRequest("Bu kullanıcı adı zaten mevcut");

            var emailExists = await _context.Users
                .AnyAsync(x => x.Email == request.Email);

            if (emailExists)
                return BadRequest("Bu email zaten kayıtlı");

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = request.Username,
                Email = request.Email,
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("Kayıt başarılı");
        }

        // =========================
        // LOGIN
        // =========================
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Username == request.Username);

            if (user == null)
                return Unauthorized("Kullanıcı bulunamadı");

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

            if (!isPasswordValid)
                return Unauthorized("Şifre yanlış");

            var token = _jwtService.GenerateToken(user.Id, user.Username);
            return Ok(new { token });
        }
        
        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            var userId = User.FindFirst("userId")?.Value;
            var username = User.FindFirst("username")?.Value;
            return Ok(new { userId, username });
        }
    }
}
