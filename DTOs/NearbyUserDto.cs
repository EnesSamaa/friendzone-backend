namespace friendzone_backend.DTOs
{
    public class NearbyUserDto
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public double DistanceKm { get; set; }
    }
}