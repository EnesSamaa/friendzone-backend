using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using friendzone_backend.Data;
using friendzone_backend.DTOs;
using friendzone_backend.Entities;
using friendzone_backend.Services;

namespace friendzone_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LocationController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly CurrentUserService _currentUser;
        private readonly IMatchingService _matchingService;

        public LocationController(AppDbContext context, CurrentUserService currentUser, IMatchingService matchingService)
        {
            _context = context;
            _currentUser = currentUser;
            _matchingService = matchingService;
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateLocation(UpdateLocationDto dto)
        {
            var userId = _currentUser.UserId;

            var location = await _context.Locations
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (location == null)
            {
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
                location.Latitude = dto.Latitude;
                location.Longitude = dto.Longitude;
                location.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Konum güncellendi", dto.Latitude, dto.Longitude });
        }

        [HttpGet("nearby")]
        public async Task<IActionResult> GetNearbyUsers([FromQuery] double radiusKm = 2)
        {
            var userId = _currentUser.UserId;
            var users = await _matchingService.GetNearbyUsersAsync(userId, radiusKm);
            return Ok(users);
        }
    }
}