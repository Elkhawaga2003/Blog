using Blog.Data;
using Blog.Migrations;
using Blog.Models;
using Blog.Repository;
using System.Security.Claims;

namespace Blog.UnitOfWorks
{
    public class UnitOfWork:IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Posts = new BaseRepository<Post>(_context);
            Comments = new BaseRepository<Comment>(_context);
            PostLikes = new BaseRepository<PostLike>(_context);
            FriendShips = new BaseRepository<FriendShip>(_context);
           
        }

        public IBaseRepository<Post> Posts { get; private set; }
        public IBaseRepository<Comment> Comments { get; private set; }
        public IBaseRepository<PostLike> PostLikes { get; private set; }
        public IBaseRepository<FriendShip> FriendShips { get; private set; }


        public async Task CommitTransaction()
        {
            await _context.Database.CommitTransactionAsync();
        }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }

        public async Task RollbackTransaction()
        {
            await _context.Database.RollbackTransactionAsync();
        }

        public async Task StartTransaction()
        {
            await _context.Database.BeginTransactionAsync();
        }
    }

}
