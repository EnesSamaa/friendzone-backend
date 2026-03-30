namespace friendzone_backend.Entities
{
    public enum InviteStatus
    {
        Pending,
        Accepted,
        Rejected
    }

    public class Invite
    {
        public Guid Id { get; set; }

        public Guid SenderId { get; set; }
        public User Sender { get; set; } = null!;

        public Guid ReceiverId { get; set; }
        public User Receiver { get; set; } = null!;

        public InviteStatus Status { get; set; } = InviteStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}