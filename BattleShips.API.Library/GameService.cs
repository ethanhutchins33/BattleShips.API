using BattleShips.API.Data.Access;
using BattleShips.API.Data.Models;

namespace BattleShips.API.Library;

public class GameService : IGameService
{
    private readonly IRepository<Game> _gameRepository;
    private readonly IRepository<Board> _boardRepository;
    private readonly IRepository<Player> _playerRepository;
    private readonly IRepository<Ship> _shipRepository;
    private readonly IRepository<ShipType> _shipTypeRepository;
    private readonly IRepository<Shot> _shotRepository;

    public GameService(
        IRepository<Game> gameRepository, 
        IRepository<Board> boardRepository,
        IRepository<Player> playerRepository,
        IRepository<Ship> shipRepository,
        IRepository<ShipType> shipTypeRepository,
        IRepository<Shot> shotRepository)
    {
        _gameRepository = gameRepository;
        _boardRepository = boardRepository;
        _playerRepository = playerRepository;
        _shipRepository = shipRepository;
        _shipTypeRepository = shipTypeRepository;
        _shotRepository = shotRepository;
    }

    public async Task<Game?> SetupNewGameAsync(int playerId)
    {
        var player = await _playerRepository.GetAsync(playerId);

        if (player == null)
        {
            throw new NullReferenceException(
                $"{nameof(SetupNewGameAsync)}: No Player found with Player ID: {playerId}");

        }

        var newGame = await _gameRepository.AddAsync(new Game
        {
            DateCreated = DateTime.Now,
            Player1Id = player.Id,
            GameCode = GenerateRandomCode(),
        });

        return newGame;
    }

    public async Task<Game?> AddPlayerToGameAsync(int joiningPlayerId, int gameId)
    {
        var game = await _gameRepository.GetAsync(gameId);

        if (game == null)
        {
            throw new NullReferenceException($"No game found with Game Id: {gameId}");
        }

        var player = await _playerRepository.GetAsync(joiningPlayerId);

        if (player == null)
        {
            throw new NullReferenceException($"No player found with Player ID: {joiningPlayerId}");
        }

        if (game.Player1Id == player.Id || game.Player2Id == player.Id)
        {
            return game;
        }

        if (game.Player2Id == null)
        {
            game.Player2Id = player.Id;
            await _gameRepository.UpdateAsync(game);
        }

        return game;
    }

    public Game? GetGameByGameCode(string gameCode)
    {
        var games = _gameRepository.GetAll();

        if(games == null)
        {
            throw new NullReferenceException("No games were found");
        }
        
        var game = games.FirstOrDefault(x => x.GameCode == gameCode);
        
        if (game == null)
        {
            throw new NullReferenceException($"Game with gameCode: {gameCode} does not exist");
        }

        return game;
    }

    public async Task<Board?> NewBoardAsync(int playerId, int gameId)
    {
        var board = GetBoard(playerId, gameId);
        if (board != null) return board;

        var newBoard = await _boardRepository.AddAsync(new Board
        {
            PlayerId = playerId,
            GameId = gameId,
        });

        return newBoard;
    }

    public async Task AddShipsToBoardAsync(string[,] ships, string gameCode, int playerId)
    {
        //TODO: Validate ships
        var game = GetGameByGameCode(gameCode);

        if(game == null)
        {
            throw new NullReferenceException($"No game found with gamecode: {gameCode}");
        }

        var board = GetBoard(game.Id, playerId);

        if(board == null)
        {
            throw new NullReferenceException($"Board not found for player: {playerId} and game: {gameCode}");
        }

        List<Ship> shipList = new();

        for (int x = 0; x < ships.GetLength(0); x++)
        {
            for (int y = 0; y < ships.GetLength(1); y++)
            {
                if (ships[x, y] == "S")
                {
                    shipList.Add(new Ship
                    {
                        BoardId = board.Id,
                        PosX = x,
                        PosY = y,
                        IsVertical = false, //TODO fix hardcoded value
                        ShipTypeId = 1 //TODO fix hardcoded value
                    });
                }
            }
        }

        foreach (var ship in shipList)
        {
            await _shipRepository.AddAsync(ship);
        }
    }

    public async Task<Player?> GetOpponentAsync(int gameId, int hostPlayerId)
    {
        var game = await _gameRepository.GetAsync(gameId);

        if(game == null) return null; 

        var p1 = game.Player1Id;
        var p2 = game.Player2Id;

        if (p1 == hostPlayerId)
        {
            return await _playerRepository.GetAsync(p2);
        } 
        
        if(p2 == hostPlayerId)
        {
            return await _playerRepository.GetAsync(p1);
        }

        return null;
    }

    public Board? GetBoard(int gameId, int playerId)
    {
        var boards = _boardRepository.GetAll();

        if (boards == null)
        {
            Console.WriteLine("No boards were found");
            return null;
        }

        var board = boards.SingleOrDefault(b => b.PlayerId == playerId && b.GameId == gameId);

        if (board == null)
        {
            Console.WriteLine($"No Board Found for Player Id: {playerId}");
            return null;
        }

        return board;

    }

    public Shot? GetLastShot(int gameId)
    {
        var boards = _boardRepository.GetAll()!
            .Where(b => b.GameId == gameId)
            .ToList();

        if (boards == null)
        {
            throw new NullReferenceException($"{nameof(GetLastShot)}: No boards found in database with Game Id: {gameId}");
        }

        var shots = new List<Shot>();

        foreach (var board in boards)
        {
            foreach(var shot in 
                    _shotRepository.GetAll()!
                    .Where(s => s.BoardId == board.Id))
            {
                shots.Add(shot);
            }
        }

        if (!shots.Any())
        {
            return null;
        }

        var result = shots.OrderBy((shot) => shot.Id).LastOrDefault();

        if (result == null)
        {
            throw new NullReferenceException($"{nameof(GetLastShot)}: No shot found in database");
        }

        return result;

    }

    

    public async Task<Shot?> CheckShotAsync(int boardId, int x, int y)
    {

        var board = await _boardRepository.GetAsync(boardId);
        if (board == null)
        {
            throw new NullReferenceException($"{nameof(CheckShotAsync)}: No board found with board Id: {boardId}");
        }

        var ships = GetShipsByBoardId(boardId);
        if (!ships.Any())
        {
            throw new NullReferenceException($"{nameof(CheckShotAsync)}: No ships found on board with board Id: {boardId}");
        }

        Shot? result;

        if (ships.Any(ship => ship.PosX == x && ship.PosY == y))
        {
            result = await _shotRepository.AddAsync(
                new Shot
                {
                    BoardId = boardId,
                    X = x,
                    Y = y,
                    ShotStatus = "hit",
                });
        } 
        else
        {
            result = await _shotRepository.AddAsync(
                new Shot
                {
                    BoardId = boardId,
                    X = x,
                    Y = y,
                    ShotStatus = "missed",
                });
        }
        if (result == null)
        {
            throw new NullReferenceException($"{nameof(CheckShotAsync)}: Could not add new shot to shot repo");
        }

        return result;

    }

    public async Task ReadyUpAsync(string gameCode, int playerId) //TODO switch parameter gameCode to boardId
    {
        var game = GetGameByGameCode(gameCode);

        if(game == null)
        {
            throw new NullReferenceException($"{nameof(ReadyUpAsync)}: No game found with gameCode: {gameCode}");
        }

        var board = GetBoard(game.Id, playerId);

        if (board == null)
        {
            throw new NullReferenceException($"{nameof(ReadyUpAsync)}: No board found with Game Id: {game.Id} and Player Id: {playerId}");
        }

        board.IsReady = true;

        await _boardRepository.UpdateAsync(board);
    }

    public string[,] GetShipsMatrix(int boardId)
    {
        var ships = GetShipsByBoardId(boardId);

        var matrix = new string[7, 7];

        for (var i = 0; i < 7; i++)
        {
            for (var j = 0; j < 7; j++)
            {
                matrix[i, j] = "";
            }
        }

        ships.ForEach(ship =>
        {
            matrix[ship.PosX, ship.PosY] = "S";
        });

        return matrix;
    }

    public async Task<bool> GetLobbyReadyStatusAsync(int gameId)
    {
        var game = await _gameRepository.GetAsync(gameId);
        if (game == null)
        {
            throw new NullReferenceException($"{nameof(GetLobbyReadyStatusAsync)}: No game found with Game Id: {gameId}");
        }

        if (game.Player2Id == null)
        {
            return false;
        }

        var p1ReadyStatus = GetBoard(game.Id, game.Player1Id)!.IsReady;

        var p2Board = GetBoard(game.Id, (int)game.Player2Id);

        if (p2Board == null)
        {
            throw new NullReferenceException($"{nameof(GetLobbyReadyStatusAsync)}: No board found with Game Id: {game.Id} and Player Id: {game.Player2Id}");
        }

        var p2ReadyStatus = p2Board.IsReady;

        return p1ReadyStatus && p2ReadyStatus;
    }

    //public async Task<int> GetPlayerTurnBoardId(int gameId)
    //{
    //    var game = await _gameRepository.GetAsync(gameId);

    //    if (game == null)
    //    {
    //        throw new NullReferenceException($"{nameof(GetPlayerTurnBoardId)}: No game found with Game Id: {gameId}");
    //    }

    //    var p1Board = GetBoard(gameId, game.Player1Id);
    //    if (game.Player2Id != null)
    //    {
    //        var p2Board = GetBoard(gameId, (int)game.Player2Id);
    //    }

    //    var lastShot = await GetLastShot(gameId);

    //    if (lastShot == null)
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
        var game = await _gameRepository.GetAsync(gameId);

        if (game == null)
        {
            throw new NullReferenceException();
        }

        game.DateStarted = DateTime.Now;

        await _gameRepository.UpdateAsync(game);

        return game.DateStarted;
    }

    private static string GenerateRandomCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var stringChars = new char[8];
        var random = new Random();

        for (var i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[random.Next(chars.Length)];
        }

        return new string(stringChars);
    }

    private List<Ship> GetShipsByBoardId(int boardId)
    {
        var ships = _shipRepository.GetAll();

        if (ships == null)
        {
            throw new NullReferenceException($"{nameof(GetShipsByBoardId)}: No ships found");
        }

        return ships.Where(Ship => Ship.BoardId == boardId).ToList();
    }
}
