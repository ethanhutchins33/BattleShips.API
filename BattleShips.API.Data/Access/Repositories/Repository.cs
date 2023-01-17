using Microsoft.EntityFrameworkCore;

namespace BattleShips.API.Data.Access.Repositories;

public abstract class Repository<TEntity, TContext> : IRepository<TEntity>
where TEntity : class, IEntity
where TContext : DbContext
{
    private readonly TContext _context;

    protected Repository(TContext context)
    {
        _context = context;
    }

    public async Task<TEntity?> AddAsync(TEntity? entity)
    {
        try
        {
            if (entity != null)
            {
                _context.Set<TEntity>().Add(entity);
                await _context.SaveChangesAsync();
                var newRecordId = entity.Id;
                return entity;
            }
        }
        catch
        {
            return null;
        }
        
        return null;
    }

    public IQueryable<TEntity>? GetAll()
    {
        try
        {
            return _context.Set<TEntity>().AsQueryable();
        }
        catch
        {
            return null;
        }
    }

    public async Task<TEntity?> GetAsync(int? id)
    {
        try
        {
            if (id != null)
            {
                return await _context.Set<TEntity>().FindAsync(id);
            }
        }
        catch
        {
            return null;
        }

        return null;
    }

    public async Task<TEntity?> UpdateAsync(TEntity? entity)
    {
        try
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }
        catch
        {
            return null;
        }
    }

    public async Task<TEntity?> DeleteAsync(int? id)
    {
        try
        {
            if (id != null)
            {
                var entity = await _context.Set<TEntity>().FindAsync(id);

                if (entity != null)
                {
                    return entity;
                }

                _context.Set<TEntity>().Remove(entity);
                await _context.SaveChangesAsync();

                return entity;
            }
        }
        catch
        {
            return null;
        }

        return null;
    }

}
