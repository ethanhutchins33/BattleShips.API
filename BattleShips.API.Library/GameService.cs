using BattleShips.API.Data.Access;
using BattleShips.API.Data.Models;

namespace BattleShips.API.Library;

public class GameService(
    IRepository<Game> gameRepository,
    IRepository<Board> boardRepository,
    IRepository<Player> playerRepository,
    IRepository<Ship> shipRepository,
    IRepository<Shot> shotRepository)
    : IGameService
{
    public async Task<Game?> SetupNewGameAsync(int playerId)
    {
        var player = await playerRepository.GetAsync(playerId);

        if (player is null)
            throw new ArgumentNullException(
                $"{nameof(SetupNewGameAsync)}: No Player found with Player ID: {playerId}");

        return await gameRepository.AddAsync(new Game
        {
            DateCreated = DateTime.Now,
            Player1Id = player.Id,
            GameCode = GenerateRandomCode()
        });
    }

    public async Task AddPlayerToGameAsync(int joiningPlayerId, Game game)
    {
        var player = await playerRepository.GetAsync(joiningPlayerId);

        ArgumentNullException.ThrowIfNull(player);

        if (game.Player2Id is null)
        {
            game.Player2Id = player.Id;
            await gameRepository.UpdateAsync(game);
        }
    }

    public Game? GetGameByGameCode(string gameCode)
    {
        var games = gameRepository.GetAll();

        if (games is null)
            throw new NullReferenceException("No games were found");

        var game = games.FirstOrDefault(x => x.GameCode == gameCode);

        if (game is null)
            throw new NullReferenceException(
                $"Game with gameCode: {gameCode} does not exist");

        return game;
    }

    public Task<Board> NewBoardAsync(int playerId, int gameId)
    {
        return boardRepository.AddAsync(new Board
        {
            PlayerId = playerId,
            GameId = gameId
        });
    }

    public async Task AddShipsToBoardAsync(string[,] ships, string gameCode,
        int playerId)
    {
        //TODO: Validate ships
        var game = GetGameByGameCode(gameCode);

        if (game is null)
            throw new NullReferenceException(
                $"No game found with gamecode: {gameCode}");

        var board = GetBoard(game.Id, playerId);

        if (board is null)
            throw new NullReferenceException(
                $"Board not found for player: {playerId} and game: {gameCode}");

        List<Ship> shipList = new();

        for (var x = 0; x < ships.GetLength(0); x++)
        for (var y = 0; y < ships.GetLength(1); y++)
            if (ships[x, y] == "S")
                shipList.Add(new Ship
                {
                    BoardId = board.Id,
                    PosX = x,
                    PosY = y,
                    IsVertical = false, //TODO fix hardcoded value
                    ShipTypeId = 1 //TODO fix hardcoded value
                });

        foreach (var ship in shipList) await shipRepository.AddAsync(ship);
    }

    public async Task<Player?> GetOpponentAsync(int gameId, int hostPlayerId)
    {
        var game = await gameRepository.GetAsync(gameId);

        if (game is null) return null;

        var p1 = game.Player1Id;
        var p2 = game.Player2Id;

        if (p1 == hostPlayerId) return await playerRepository.GetAsync(p2);

        if (p2 == hostPlayerId) return await playerRepository.GetAsync(p1);

        return null;
    }

    public Board GetBoard(int gameId, int playerId)
    {
        var boards = boardRepository.GetAll();

        ArgumentNullException.ThrowIfNull(boards);

        var board =
            boards.Single(b => b.PlayerId == playerId && b.GameId == gameId);

        ArgumentNullException.ThrowIfNull(board);

        return board;
    }

    public Shot? GetLastShot(int gameId)
    {
        var boards = boardRepository.GetAll()!
            .Where(b => b.GameId == gameId)
            .ToList();

        if (boards is null)
            throw new NullReferenceException(
                $"{nameof(GetLastShot)}: No boards found in database with Game Id: {gameId}");

        var shots = new List<Shot>();

        foreach (var board in boards)
        foreach (var shot in
                 shotRepository.GetAll()!
                     .Where(s => s.BoardId == board.Id))
            shots.Add(shot);

        if (!shots.Any()) return null;

        var result = shots.OrderBy(shot => shot.Id).LastOrDefault();

        if (result is null)
            throw new NullReferenceException(
                $"{nameof(GetLastShot)}: No shot found in database");

        return result;
    }


    public async Task<Shot?> CheckShotAsync(int boardId, int x, int y)
    {
        var board = await boardRepository.GetAsync(boardId);
        if (board is null)
            throw new NullReferenceException(
                $"{nameof(CheckShotAsync)}: No board found with board Id: {boardId}");

        var ships = GetShipsByBoardId(boardId).ToList();
        if (!ships.Any())
            throw new NullReferenceException(
                $"{nameof(CheckShotAsync)}: No ships found on board with board Id: {boardId}");

        Shot? result;

        if (ships.Any(ship => ship.PosX == x && ship.PosY == y))
            result = await shotRepository.AddAsync(
                new Shot
                {
                    BoardId = boardId,
                    X = x,
                    Y = y,
                    ShotStatus = "hit"
                });
        else
            result = await shotRepository.AddAsync(
                new Shot
                {
                    BoardId = boardId,
                    X = x,
                    Y = y,
                    ShotStatus = "missed"
                });

        if (result is null)
            throw new NullReferenceException(
                $"{nameof(CheckShotAsync)}: Could not add new shot to shot repo");

        return result;
    }

    public async Task
        ReadyUpAsync(string gameCode,
            int playerId) //TODO switch parameter gameCode to boardId
    {
        var game = GetGameByGameCode(gameCode);

        if (game is null)
            throw new NullReferenceException(
                $"{nameof(ReadyUpAsync)}: No game found with gameCode: {gameCode}");

        var board = GetBoard(game.Id, playerId);

        if (board is null)
            throw new NullReferenceException(
                $"{nameof(ReadyUpAsync)}: No board found with Game Id: {game.Id} and Player Id: {playerId}");

        board.IsReady = true;

        await boardRepository.UpdateAsync(board);
    }

    public string[,] GetShipsMatrix(int boardId)
    {
        var ships = GetShipsByBoardId(boardId).ToList();

        var matrix = new string[7, 7];

        for (var i = 0; i < 7; i++)
        for (var j = 0; j < 7; j++)
            matrix[i, j] = "";

        ships.ForEach(ship => { matrix[ship.PosX, ship.PosY] = "S"; });

        return matrix;
    }

    public async Task<bool> GetLobbyReadyStatusAsync(int gameId)
    {
        var game = await gameRepository.GetAsync(gameId);
        if (game is null)
            throw new NullReferenceException(
                $"{nameof(GetLobbyReadyStatusAsync)}: No game found with Game Id: {gameId}");

        if (game.Player2Id is null) return false;

        var p1ReadyStatus = GetBoard(game.Id, game.Player1Id)!.IsReady;

        var p2Board = GetBoard(game.Id, (int)game.Player2Id);

        if (p2Board is null)
            throw new NullReferenceException(
                $"{nameof(GetLobbyReadyStatusAsync)}: No board found with Game Id: {game.Id} and Player Id: {game.Player2Id}");

        var p2ReadyStatus = p2Board.IsReady;

        return p1ReadyStatus && p2ReadyStatus;
    }

    //public async Task<int> GetPlayerTurnBoardId(int gameId)
    //{
    //    var game = await _gameRepository.GetAsync(gameId);

    //    if (game is null)
    //    {
    //        throw new NullReferenceException($"{nameof(GetPlayerTurnBoardId)}: No game found with Game Id: {gameId}");
    //    }

    //    var p1Board = GetBoard(gameId, game.Player1Id);
    //    if (game.Player2Id != null)
    //    {
    //        var p2Board = GetBoard(gameId, (int)game.Player2Id);
    //    }

    //    var lastShot = await GetLastShot(gameId);

    //    if (lastShot is null)
    //    {
    //        return game.Player1Id;
    //    }

    //    var board = _boardRepository.GetAsync(lastShot.BoardId);

    //    if (lastShot.BoardId == board.Id)
    //    {

    //    }
    //}

    public async Task<DateTime> SetGameStartedDateTimeAsync(int gameId)
    {
        var game = await gameRepository.GetAsync(gameId);

        if (game is null) throw new NullReferenceException();

        game.DateStarted = DateTime.Now;

        await gameRepository.UpdateAsync(game);

        return game.DateStarted;
    }

    private static string GenerateRandomCode()
    {
        const string chars =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var stringChars = new char[8];
        var random = new Random();

        for (var i = 0; i < stringChars.Length; i++)
            stringChars[i] = chars[random.Next(chars.Length)];

        return new string(stringChars);
    }

    private IEnumerable<Ship> GetShipsByBoardId(int boardId)
    {
        var ships = shipRepository.GetAll();

        if (ships is null)
            throw new NullReferenceException(
                $"{nameof(GetShipsByBoardId)}: No ships found");

        return ships.Where(ship => ship.BoardId == boardId);
    }
}