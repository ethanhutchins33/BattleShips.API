using BattleShips.API.Data.Models;

namespace BattleShips.API.Library;

public interface IGameService
{
    Task<Game?> SetupNewGameAsync(int playerId);
    Task<Game?> AddPlayerToGameAsync(int joiningPlayerId, int gameId);
    Task<Board?> NewBoardAsync(int playerId, int gameId);
    Task AddShipsToBoardAsync(string[,] ships, string gameCode, int playerId);
    Game? GetGameByGameCode(string gameCode);
    Task<Player?> GetOpponentAsync(int gameId, int hostPlayerId);
    Board? GetBoard(int gameId, int playerId);
    Task<Shot?> GetLastShotAsync(int gameId);
    Task<Shot?> CheckShot(int boardId, int x, int y);
    Task ReadyUpAsync(string gameCode, int playerId);
    string[,] GetShipsMatrix(int boardId);
    Task<bool> GetLobbyReadyStatusAsync(int gameId);
    //Task<int> GetPlayerTurnBoardId(int gameId);
    Task<DateTime> SetGameStartedDateTime(int gameId);
}