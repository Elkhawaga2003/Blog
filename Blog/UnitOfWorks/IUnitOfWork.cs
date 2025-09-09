using Blog.Migrations;
using Blog.Models;
using Blog.Repository;
using System.Security.Claims;

namespace Blog.UnitOfWorks
{
    public interface IUnitOfWork:IDisposable
    {
        public int Complete();
        public Task StartTransaction();
        public Task CommitTransaction();
        public Task RollbackTransaction();
        public IBaseRepository<Post> Posts { get; }
        public IBaseRepository<Comment> Comments { get; }
        public IBaseRepository<PostLike> PostLikes { get;  }
        public IBaseRepository<FriendShip> FriendShips { get;  }


    }
}
