using BattleShips.API.Data.Access;
using BattleShips.API.Data.Models;
using Microsoft.EntityFrameworkCore;

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

        if (player == null) return null;
        
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

        if (game.Player1Id == player.Id)
        {
            return game;
        }
        else
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
        else if(p2 == hostPlayerId)
        {
            return await _playerRepository.GetAsync(p1);
        }
        else
        {
            return null;
        }
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

    public async Task<Shot?> GetLastShotAsync(string gameCode, int playerId)
    {
        var game = GetGameByGameCode(gameCode);

        if (game == null)
        {
            throw new NullReferenceException($"{nameof(GetLastShotAsync)}: No game found with gameCode: {gameCode}");
        }

        var boards = _boardRepository.GetAll().Where(b => b.GameId == game.Id).ToList();

        if (boards == null)
        {
            throw new NullReferenceException($"{nameof(GetLastShotAsync)}: No boards found in database with Game Id: {game.Id}");
        }

        var shots = new List<Shot>();

        foreach (var board in boards)
        {
            await _shotRepository.GetAll()
                .Where(s => s.BoardId == board.Id)
                .ForEachAsync(s => shots.Add(s));
        }

        if (!shots.Any())
        {
            throw new NullReferenceException($"{nameof(GetLastShotAsync)}: No shots found in database with Board Ids: {boards[0].Id} & {boards[1].Id}");
        }

        var result = shots.OrderBy((shot) => shot.Id).LastOrDefault();

        if (result == null)
        {
            throw new NullReferenceException($"{nameof(GetLastShotAsync)}: No shot found in database with shot Id: {result.Id}");
        }

        return result;

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

    public async Task<Shot?> CheckShot(int boardId, int X, int Y)
    {

        var board = await _boardRepository.GetAsync(boardId);
        if (board == null)
        {
            throw new NullReferenceException($"{nameof(CheckShot)}: No board found with board Id: {boardId}");
        }

        var ships = GetShipsByBoardId(boardId);
        if (!ships.Any())
        {
            throw new NullReferenceException($"{nameof(CheckShot)}: No ships found on board with board Id: {boardId}");
        }

        var result = new Shot();

        if (ships.Any(ship => ship.PosX== X && ship.PosY == Y))
        {
            result = await _shotRepository.AddAsync(
                new Shot
                {
                    BoardId = boardId,
                    X = X,
                    Y = Y,
                    ShotStatus = "hit",
                });
        } 
        else
        {
            result = await _shotRepository.AddAsync(
                new Shot
                {
                    BoardId = boardId,
                    X = X,
                    Y = Y,
                    ShotStatus = "missed",
                });
        }
        if (result == null)
        {
            throw new NullReferenceException($"{nameof(CheckShot)}: Could not add new shot to shot repo");
        }

        return result;

    }

    public async Task ReadyUpAsync(string gameCode, int playerId)
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

        var matrix = new string[7,7];

        ships.ForEach(ship =>
        {
            matrix[ship.PosX, ship.PosY] = "S";
        });

        return matrix;
    }

    public async Task<bool> GetLobbyReadyStatusAsync(int gameId, int hostId)
    {
        var game = await _gameRepository.GetAsync(gameId);
        if (game == null)
        {
            throw new NullReferenceException($"{nameof(GetLobbyReadyStatusAsync)}: No game found with Game Id: {gameId}");
        }


        var boards = _boardRepository.GetAll();
        if (boards == null)
        {
            throw new NullReferenceException($"{nameof(GetLobbyReadyStatusAsync)}: No boards found");
        }

        var p1ReadyStatus = boards.FirstOrDefault(b => b.GameId == gameId && b.PlayerId == game.Player1Id)!.IsReady;
        var p2 = boards.FirstOrDefault(b => b.GameId == gameId && b.PlayerId == game.Player2Id);
        var p2ReadyStatus = false;

        if (p2 == null)
        {
            return false;
        }
        else
        {
            p2ReadyStatus = p2.IsReady;
        }

        return p1ReadyStatus && p2ReadyStatus;
    }

    //public async Task<string> CheckShot(int boardId, int rowNumber, char cellValue)
    //{
    //    //TODO validate if shot hit a ship on board
    //    //var allShips = _shipRepository.GetAll();
    //    //var ships = allShips.Select(x => x.BoardId == boardId);

    //    return 
    //}
}
