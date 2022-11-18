
using BattleShips.API.Data.Models;

namespace BattleShips.API.Data.DataAccess;
public interface IGameRepository
{
    Task<Game> GetGameAsync(int id);
    Task<Game> CreateGameAsync(Game game);
    Task<Game> UpdateGameAsync(Game game);
    Task<(bool, string)> DeleteGameAsync(Game game);
    

}
