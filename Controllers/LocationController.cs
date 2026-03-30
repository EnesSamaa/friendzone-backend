using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using friendzone_backend.Data;
using friendzone_backend.DTOs;
using friendzone_backend.Entities;
using System.Security.Claims;

namespace friendzone_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LocationController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LocationController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateLocation(UpdateLocationDto dto)
        {
            var userIdStr = User.FindFirst("userId")?.Value;
            if (userIdStr == null) return Unauthorized();

            var userId = Guid.Parse(userIdStr);

            var location = await _context.Locations
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (location == null)
            {
                // İlk kez konum kaydediliyor
                location = new Location
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Latitude = dto.Latitude,
                    Longitude = dto.Longitude,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.Locations.Add(location);
            }
            else
            {
                // Mevcut konum güncelleniyor
                location.Latitude = dto.Latitude;
                location.Longitude = dto.Longitude;
                location.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Konum güncellendi", dto.Latitude, dto.Longitude });
        }
    }
}