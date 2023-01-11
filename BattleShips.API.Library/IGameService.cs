using BattleShips.API.Data.Models;

namespace BattleShips.API.Library;

public interface IGameService
{
    Task<Game?> SetupNewGameAsync(int playerId); //TODO rename to async
    Task<Game?> AddPlayerToGameAsync(int joiningPlayerId, int gameId);
    Task<Board?> NewBoardAsync(int playerId, int gameId);
    Task AddShipsToBoardAsync(string[,] ships, string gameCode, int playerId);
    Game? GetGameByGameCode(string gameCode);
    Task<int?> GetOpponentIdAsync(int hostPlayerId, int gameId);
    int? GetOpponentBoardId(int? hostPlayerId, int gameId);
}