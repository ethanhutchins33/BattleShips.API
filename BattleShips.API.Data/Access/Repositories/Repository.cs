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

    public async Task<TEntity> Add(TEntity entity)
    {
        try
        {
            _context.Set<TEntity>().Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        catch
        {
            return null;
        }

    }

    public async Task<List<TEntity>> GetAll()
    {
        return await _context.Set<TEntity>().ToListAsync();
    }

    public async Task<TEntity> Get(int id)
    {
        try
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }
        catch
        {
            return null;
        }

    }



    public async Task<TEntity> Update(TEntity entity)
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

    public async Task<TEntity> Delete(int id)
    {
        try
        {
            var entity = await _context.Set<TEntity>().FindAsync(id);

            if (entity == null)
            {
                return null;
            }

            _context.Set<TEntity>().Remove(entity);
            await _context.SaveChangesAsync();

            return entity;
        }
        catch
        {
            return null;
        }

    }

}
