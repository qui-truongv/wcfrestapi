using System.Linq.Expressions;

namespace QMS.Core.Interfaces.Repositories;

public interface IGenericRepository<T> where T : class
{
    // Queries
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

    // Commands
    Task<T> AddAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entities);
    void Update(T entity);
    void UpdateRange(IEnumerable<T> entities);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);

    // Count
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
}