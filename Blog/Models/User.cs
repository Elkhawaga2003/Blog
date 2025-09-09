using Microsoft.AspNetCore.Identity;

namespace Blog.Models
{
    public class User:IdentityUser
    {
        public string Name { get; set; }
        public string? ProfileUrl { get; set; }
        public string? Bio { get; set; }
        public string? ImageUrl { get; set; }
        public ICollection<Post> Posts { get; set; } = new HashSet<Post>();
        public ICollection<PostLike> PostLikes { get; set; } = new List<PostLike>();

        public ICollection<FriendShip> FriendRequestSend { get; set; } = new HashSet<FriendShip>();
        public ICollection<FriendShip> FriendRequestRecive { get; set; } = new HashSet<FriendShip>();
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new HashSet<RefreshToken>();
    }
}
