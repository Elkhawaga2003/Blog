namespace Blog.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Content { get; set; }
        
        public double Likes { get; set; } = 0;
        public User User { get; set; }
        public string UserId { get; set; }
        public ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? ImageUrl { get; set; }
        public ICollection<PostLike> PostLikes { get; set; } = new List<PostLike>();

    }
}
