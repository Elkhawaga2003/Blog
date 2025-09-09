using Blog.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Blog.Repository
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        #region constractor
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;
        public BaseRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }
        #endregion

        #region Implementation
        public IEnumerable<T> FindItem(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate).ToList();
        }

        public IEnumerable<T> AsQuearable()
        {
            return _dbSet.AsQueryable();
        }

        public async Task AddItemAsync(T item)
        {
            await _dbSet.AddAsync(item);
        }

        public void DeleteItem(T item)
        {
            _dbSet.Remove(item);
        }

        public IEnumerable<T> FindItems(Expression<Func<T, bool>> predicate, List<string> includes)
        {
            IQueryable<T> query = _dbSet;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return query.Where(predicate).ToList();
        } 
        public IEnumerable<T> Include(int skip,int pageSize, List<string> includes)
        {
            IQueryable<T> query = _dbSet;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return query.Skip(skip).Take(pageSize).ToList();
        }


        public async Task<IEnumerable<T>> GetAllItemsAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> GetItemByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public void UpdateItem(T item)
        {
            _dbSet.Update(item);
        }
        #endregion
    }
}
