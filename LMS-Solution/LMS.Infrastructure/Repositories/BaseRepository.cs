using LMS.Domain.Repositories;
using LMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LMS.Infrastructure.Repositories;

internal class BaseRepository<T> : IBaseRepository<T> where T : class
{
    private readonly AppDbContext _context;

    public BaseRepository(AppDbContext context)
    {
        _context = context;
    }

    public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate) => await _context.Set<T>().AnyAsync(predicate);
    
    public virtual async Task<int> CountAsync() => await _context.Set<T>().CountAsync();
    
    public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate) => await _context.Set<T>().CountAsync(predicate);
    
    public virtual void Delete(T entity) => _context.Set<T>().Remove(entity);

    public virtual async Task<IEnumerable<T>> FilterAsync(Expression<Func<T, bool>> predicate, string[] includeProperties = null)
    {
        var query = _context.Set<T>().Where(predicate);
        if (includeProperties != null)
        {
            foreach (var property in includeProperties)
            {
                query = query.Include(property);
            }
        }

        return await query
            .AsNoTracking()
            .ToListAsync();
    }

    public virtual async Task<IEnumerable<T>> FilterAsync(Expression<Func<T, bool>> predicate, int skip, int take, string[] includeProperties = null)
    {
        var query = _context.Set<T>().Where(predicate);

        if (includeProperties != null)
        {
            foreach (var property in includeProperties)
            {
                query = query.Include(property);
            }
        }

        query = query
            .Skip(skip)
            .Take(take);

        return await query
            .AsNoTracking()
            .ToListAsync();
    }

    public virtual async Task<IEnumerable<T>> FilterAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderBy, bool isDescending = false, string[] includeProperties = null)
    {
        var query = _context.Set<T>().Where(predicate);

        if (includeProperties != null)
        {
            foreach (var property in includeProperties)
            {
                query = query.Include(property);
            }
        }

        if (isDescending)
            query = query.OrderByDescending(orderBy);
        else
            query = query.OrderBy(orderBy);

        return await query
            .AsNoTracking()
            .ToListAsync();
    }

    public virtual async Task<IEnumerable<T>> FilterAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderBy, bool isDescending, int skip, int take, string[] includeProperties = null)
    {
        var query = _context.Set<T>().Where(predicate);

        if(includeProperties != null)
        {
            foreach(var property in includeProperties)
            {
                query = query.Include(property);
            }
        }

        if (isDescending)
            query = query.OrderByDescending(orderBy);
        else
            query = query.OrderBy(orderBy);

        query = query
            .Skip(skip)
            .Take(take);

        return await query
            .AsNoTracking()
            .ToListAsync();
    }

    public virtual async Task<T?> GetByIdAsync(object id) => await _context.Set<T>().FindAsync(id);
    public virtual async Task InsertAsync(T entity) => await _context.Set<T>().AddAsync(entity);
    public virtual void Update(T entity) => _context.Set<T>().Update(entity);
}
