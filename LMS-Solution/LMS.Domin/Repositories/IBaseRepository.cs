using System.Linq.Expressions;

namespace LMS.Domin.Repositories;

public interface IBaseRepository<T> where T : class
{
    Task<T> InsertAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<T?> GetByIdAsync(object id);
    Task<IEnumerable<T>> FilterAsync(Expression<Func<T, bool>> predicate);
    Task<IEnumerable<T>> FilterAsync(
        Expression<Func<T, bool>> predicate,
        int skip,
        int take);
    Task<IEnumerable<T>> FilterAsync(
        Expression<Func<T, bool>> predicate,
        Expression<Func<T, object>> orderBy,
        bool isDescending = false);
    Task<IEnumerable<T>> FilterAsync(
        Expression<Func<T, bool>> predicate,
        Expression<Func<T, object>> orderBy,
        bool isDescending,
        int skip,
        int take);
    Task<int> CountAsync();
    Task<int> CountAsync(Expression<Func<T, bool>> predicate);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
}
