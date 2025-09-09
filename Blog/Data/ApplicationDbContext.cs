using Blog.Models;
using Blog.Repository;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Blog.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<FriendShip>().
                HasOne(f => f.Requester)
                .WithMany(u => u.FriendRequestSend)
                .HasForeignKey(f => f.RequesterId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<FriendShip>().
                HasOne(f => f.Accepter)
                .WithMany(u => u.FriendRequestRecive)
                .HasForeignKey(f => f.AccepterId)
                .OnDelete(DeleteBehavior.Restrict);
        }
        public DbSet<User> Users { get; set; }
        public DbSet<FriendShip> FriendShips { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public IBaseRepository<PostLike> PostLikes { get; private set; }

    }

}
