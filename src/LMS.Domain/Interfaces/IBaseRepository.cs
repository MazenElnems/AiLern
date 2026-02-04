using System.Linq.Expressions;

namespace LMS.Domain.Repositories;

public interface IBaseRepository<T> where T : class
{
    Task InsertAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task<T?> GetByIdAsync(object id);
    Task<T?> GetAsync(Expression<Func<T, bool>> match);
    Task<T?> GetAsync(Expression<Func<T, bool>> match, string[] includeProperties = null);
    Task<IEnumerable<T>> FilterAsync(
        Expression<Func<T, bool>> predicate,
        string[] includeProperties = null);
    Task<IEnumerable<T>> FilterAsync(
        Expression<Func<T, bool>> predicate,
        int skip,
        int take,
        string[] includeProperties = null);
    Task<IEnumerable<T>> FilterAsync(
        Expression<Func<T, bool>> predicate,
        Expression<Func<T, object>> orderBy,
        bool isDescending = false,
        string[] includeProperties = null);
    Task<IEnumerable<T>> FilterAsync(
        Expression<Func<T, bool>> predicate,
        Expression<Func<T, object>> orderBy,
        bool isDescending,
        int skip,
        int take,
        string[] includeProperties = null);
    Task<int> CountAsync();
    Task<int> CountAsync(Expression<Func<T, bool>> predicate);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellation);

}
