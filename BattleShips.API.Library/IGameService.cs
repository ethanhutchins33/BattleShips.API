using BattleShips.API.Data.Models;

namespace BattleShips.API.Library;

public interface IGameService
{
    void SetupNewGame(Player player);
    Task<Game> CreateGame(Player player);
    Task<Game?> AddPlayerToGame(Player player, int gameId);
    Task<Board> AddBoard(Player player, Game game);
    Task AddShipToBoard(Ship ship, int gameId, int playerId);
}