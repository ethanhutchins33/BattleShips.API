using BattleShips.API.Data.Models;

namespace BattleShips.API.Data.DataAccess;

public interface IGameRepository
{
    Task<Game> GetGameAsync(int id);
    Task<Game> AddGameAsync(Game game);
    Task<Game> UpdateGameAsync(Game game);
    Task<(bool, string)> DeleteGameAsync(Game game);

    Task<Board> GetBoardAsync(int id);
    Task<Board> AddBoardAsync(Board board);
    Task<Board> UpdateBoardAsync(Board board);
    Task<Board> DeleteBoardAsync(Board board);

    Task<List<Player>> GetPlayersAsync();
    Task<Player> GetPlayerAsync(int id);
    Task<Player> AddPlayerAsync(Player player);
    Task<Player> UpdatePlayerAsync(Player player);

    Task<Ship> GetShipAsync(int id);
    Task<Ship> AddShipAsync(Ship ship);

    Task<ShipType> GetShipTypeAsync(int id);
}
