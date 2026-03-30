using friendzone_backend.Data;
using friendzone_backend.DTOs;
using Microsoft.EntityFrameworkCore;

namespace friendzone_backend.Services
{
    public class MatchingService : IMatchingService
    {
        private readonly AppDbContext _context;

        public MatchingService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<NearbyUserDto>> GetNearbyUsersAsync(Guid currentUserId, double radiusKm)
        {
            // Sadece son 5 dakika içinde güncellenen konumlar (aktif kullanıcılar)
            var freshnessLimit = DateTime.UtcNow.AddMinutes(-5);

            var locations = await _context.Locations
                .Include(l => l.User)
                .Where(l => l.UserId != currentUserId &&
                            l.UpdatedAt >= freshnessLimit)
                .ToListAsync();

            var currentLocation = await _context.Locations
                .FirstOrDefaultAsync(l => l.UserId == currentUserId);

            if (currentLocation == null)
                return new List<NearbyUserDto>();

            var result = locations
                .Select(l => new NearbyUserDto
                {
                    UserId = l.UserId,
                    Username = l.User.Username,
                    DistanceKm = Haversine(
                        currentLocation.Latitude, currentLocation.Longitude,
                        l.Latitude, l.Longitude
                    )
                })
                .Where(x => x.DistanceKm <= radiusKm)
                .OrderBy(x => x.DistanceKm)
                .ToList();

            return result;
        }

        private static double Haversine(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371; // Dünya yarıçapı km
            var dLat = ToRad(lat2 - lat1);
            var dLon = ToRad(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private static double ToRad(double deg) => deg * Math.PI / 180;
    }
}