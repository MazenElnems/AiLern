using LMS.Application.Contracts.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LMS.Infrastructure.Persistence.Repositories;

internal class BaseRepository<T> : IBaseRepository<T> where T : class
{
    private readonly AppDbContext _context;
    private readonly DbSet<T> _dbSet;

    public BaseRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public IQueryable<T> Query 
        => _dbSet.AsQueryable();

    public IQueryable<T> TrackedQuery 
        => _dbSet.AsQueryable();

    public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        => await _dbSet.AnyAsync(predicate);

    public virtual async Task<bool> AnyAsync()
        =>  await _dbSet.AnyAsync();

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellation)
        => await _dbSet.AnyAsync(predicate,cancellation);

    public virtual async Task<int> CountAsync() 
        => await _dbSet.CountAsync();
    
    public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate) 
        => await _dbSet.CountAsync(predicate);
    
    public virtual void Delete(T entity) 
        => _dbSet.Remove(entity);

    public virtual async Task<IEnumerable<T>> FilterAsync(Expression<Func<T, bool>> predicate, string[] includeProperties = null)
    {
        var query = _dbSet.Where(predicate);
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
        var query = _dbSet.Where(predicate);

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
        var query = _dbSet.Where(predicate);

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
        var query = _dbSet.Where(predicate);

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

    public async Task<List<T>> GetAllAsync()
        => await GetAllAsync();

    public async Task<List<T>> GetAllAsync(string[] includeProperties = null)
    {
        var query = _dbSet.AsQueryable();
        if (includeProperties != null)
        {
            foreach (var property in includeProperties)
            {
                query = query.Include(property);
            }
        }
        return await query.AsNoTracking().ToListAsync();
    }


    public async Task<T?> GetAsync(Expression<Func<T, bool>> match)
        => await _dbSet.FirstOrDefaultAsync(match);

    public async Task<T?> GetAsync(Expression<Func<T, bool>> match, string[] includeProperties = null)
    {
        var query = _dbSet.AsQueryable();
        if (includeProperties != null)
        {
            foreach (var property in includeProperties)
            {
                query = query.Include(property);
            }
        }

        return await query.FirstOrDefaultAsync(match);
    }

    public virtual async Task<T?> GetByIdAsync(object id) 
        => await _dbSet.FindAsync(id);

    public virtual async Task InsertAsync(T entity)
        => await _dbSet.AddAsync(entity);

    public virtual async Task InsertRangeAsync(IEnumerable<T> entities)
        => await _dbSet.AddRangeAsync(entities);

    public virtual void Update(T entity) 
        => _dbSet.Update(entity);

    public void UpdateRange(T[] entities)
        => _dbSet.UpdateRange(entities);
}
