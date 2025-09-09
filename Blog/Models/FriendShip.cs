namespace Blog.Models
{
    public class FriendShip
    {
        public int Id { get; set; }
        public string RequesterId { get; set; }
        public User Requester { get; set; }
        public string AccepterId { get; set; }
        public User Accepter { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public FriendShipStatus Status { get; set; } = FriendShipStatus.Pending;
    }
    public enum FriendShipStatus
    {
        Pending,
        Accepted,
        Rejected,
        Blocked
    }
}
