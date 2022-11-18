using BattleShips.API.Data.Models;

namespace BattleShips.API.Data.DataAccess;

public interface IRepository<T> where T : class
{
    Task<List<T>> GetAll();
    Task<T> Get(int id);
    Task<T> Add(T entity);
    Task<T> Update(T entity);
    Task<(bool, string)> Delete(int id);
}
