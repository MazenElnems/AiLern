using LMS.Domin.Repositories;
using System.Linq.Expressions;

namespace LMS.Infrastructure.Repositories;

internal class BaseRepository<T> : IBaseRepository<T> where T : class
{
    public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
    {
        throw new NotImplementedException();
    }

    public Task<int> CountAsync()
    {
        throw new NotImplementedException();
    }

    public Task<int> CountAsync(Expression<Func<T, bool>> predicate)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(T entity)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<T>> FilterAsync(Expression<Func<T, bool>> predicate)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<T>> FilterAsync(Expression<Func<T, bool>> predicate, int skip, int take)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<T>> FilterAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderBy, bool isDescending = false)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<T>> FilterAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderBy, bool isDescending, int skip, int take)
    {
        throw new NotImplementedException();
    }

    public Task<T?> GetByIdAsync(object id)
    {
        throw new NotImplementedException();
    }

    public Task<T> InsertAsync(T entity)
    {
        throw new NotImplementedException();
    }

    public Task<T> UpdateAsync(T entity)
    {
        throw new NotImplementedException();
    }
}
