using System.Linq.Expressions;

namespace Blog.Repository
{
    public interface IBaseRepository<T> where T : class
    {
        public Task AddItemAsync(T item);
        public void UpdateItem(T item);
        public void DeleteItem(T item);
        public Task<T> GetItemByIdAsync(int id);
        public Task<IEnumerable<T>> GetAllItemsAsync();
        public IEnumerable<T> AsQuearable();
        public IEnumerable<T> Include(int skip, int pageSize, List<string> includes);
        public IEnumerable<T> FindItems(Expression<Func<T, bool>> predicate, List<string> includes);
        public IEnumerable<T> FindItem(Expression<Func<T, bool>> predicate);
    }
}
