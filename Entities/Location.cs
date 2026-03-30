namespace friendzone_backend.Entities
{
    public class Location
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public DateTime UpdatedAt { get; set; }

        // Navigation property
        public User User { get; set; } = null!;
    }
}