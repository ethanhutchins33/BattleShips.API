namespace BattleShips.API.Data.Access;

public interface IRepository<T> where T : class, IEntity
{
    IQueryable<T>? GetAll();
    Task<T?> GetAsync(int? id);
    Task<T> AddAsync(T entity);
    Task<T?> UpdateAsync(T? entity);
    Task<T?> DeleteAsync(int? id);
}