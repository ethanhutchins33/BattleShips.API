using BattleShips.API.Data.Models;

namespace BattleShips.API.Library;

public interface IGameService
{
    Task<Game?> SetupNewGameAsync(int playerId);
    Task<Game?> AddPlayerToGameAsync(int joiningPlayerId, int gameId);
    Task<Board?> NewBoardAsync(int playerId, int gameId);
    Task AddShipsToBoardAsync(string[,] ships, string gameCode, int playerId);
    Task<bool> GetLobbyReadyStatusAsync(int gameId);
    //Task<int> GetPlayerTurnBoardId(int gameId);
    Task<DateTime> SetGameStartedDateTimeAsync(int gameId);
    Task<Player?> GetOpponentAsync(int gameId, int hostPlayerId);
    Task<Shot?> CheckShotAsync(int boardId, int x, int y);
    Task ReadyUpAsync(string gameCode, int playerId);
    Board? GetBoard(int gameId, int playerId);
    Shot? GetLastShot(int gameId);
    string[,] GetShipsMatrix(int boardId);
    Game? GetGameByGameCode(string gameCode);
}