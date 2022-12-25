using BattleShips.API.Data.Models;

namespace BattleShips.API.Library;

public interface IGameService
{
    Task<Game?> SetupNewGame(int playerId);
    Task<Game?> AddPlayerToGame(int joiningPlayerId, int gameId);
    Task<Board?> AddBoard(int playerId, int gameId);
    Task<Ship?> AddShipToBoard(Ship ship, int gameId, int playerId);
}