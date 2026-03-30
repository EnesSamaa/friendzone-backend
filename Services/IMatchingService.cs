using friendzone_backend.DTOs;

namespace friendzone_backend.Services
{
    public interface IMatchingService
    {
        Task<List<NearbyUserDto>> GetNearbyUsersAsync(Guid currentUserId, double radiusKm);
    }
}