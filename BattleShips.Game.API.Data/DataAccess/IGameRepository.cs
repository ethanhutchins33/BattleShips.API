
namespace BattleShips.Game.API.Data.DataAccess;
public interface IGameRepository
{
    Task<Models.Game> GetGameAsync(int id);
    Task<Models.Game> CreateGameAsync(Models.Game game);
    Task<Models.Game> UpdateGameAsync(Models.Game game);
    Task<(bool, string)> DeleteGameAsync(Models.Game game);
    

}
