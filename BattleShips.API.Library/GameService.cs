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

    public GameService(
        IRepository<Game> gameRepository, 
        IRepository<Board> boardRepository,
        IRepository<Player> playerRepository,
        IRepository<Ship> shipRepository,
        IRepository<ShipType> shipTypeRepository)
    {
        _gameRepository = gameRepository;
        _boardRepository = boardRepository;
        _playerRepository = playerRepository;
        _shipRepository = shipRepository;
        _shipTypeRepository = shipTypeRepository;
    }

    public async Task<Game?> SetupNewGame(int playerId)
    {
        var player = await _playerRepository.Get(playerId);

        if (player == null) return null;
        
        var newGame = await _gameRepository.Add(new Game
        {
            DateCreated = DateTime.Now,
            Player1Id = player.Id,
            GameCode = GenerateRandomCode(),
        });

        return newGame;

    }

    public async Task<Game?> AddPlayerToGame(int joiningPlayerId, int gameId)
    {
        var game = await _gameRepository.Get(gameId);

        if (game == null)
        {
            return null;
        }

        if (game.Player1Id == joiningPlayerId || game.Player2Id == joiningPlayerId)
        {
            return game;
        }
        
        var player = await _playerRepository.Get(joiningPlayerId) ??
                     await _playerRepository.Add(new Player());

        if (player == null) return game;
        
        game.Player2Id = player.Id;
        await _gameRepository.Update(game);

        return game;
    }

    public Game? GetGameByGameCode(string gameCode)
    {
        var games = _gameRepository.GetAll();

        if(games != null)
        {
            var game = games.FirstOrDefault(x => x.GameCode == gameCode);

            return game;
        }
        return null;
    }

    public async Task<Board?> AddBoard(int playerId, int gameId)
    {
        var board = GetBoard(playerId, gameId);

        if (board != null) return board;
        
        var newBoard = await _boardRepository.Add(new Board
        {
            PlayerId = playerId,
            GameId = gameId,
        });
        return newBoard;

    }

    public async Task<Ship?> AddShipToBoard(Ship? ship, int gameId, int playerId)
    {
        await _shipRepository.Add(ship);

        var board = GetBoard(playerId, gameId);

        if (board != null)
        {
            if (ship != null)
            {
                ship.BoardId = board.Id;
            }
        }

        return await _shipRepository.Update(ship);
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

    private Board? GetBoard(int playerId, int gameId)
    {
        var boards = _boardRepository.GetAll();

        var board = boards?.FirstOrDefault(x => x.GameId == gameId && x.PlayerId == playerId);

        return board ?? null;
    }

    public async Task<int?> GetOpponentId(int hostPlayerId, int gameId)
    {
        var game = await _gameRepository.Get(gameId);

        if(game == null) return null; 

        var p1 = game.Player1Id;
        var p2 = game.Player2Id;

        if (p1 == hostPlayerId)
        {
            return p2;
        } 
        else if(p2 == hostPlayerId)
        {
            return p1;
        }
        else
        {
            return null;
        }
    }

    public int? GetOpponentBoardId(int? opponentPlayerId, int gameId)
    {
        var boards = _boardRepository.GetAll();

        if (boards == null)
        {
            Console.WriteLine("No boards were found");
            return null;
        }

        var board = boards.SingleOrDefault(b => b.PlayerId == opponentPlayerId && b.GameId == gameId);

        if (board == null)
        {
            Console.WriteLine($"No Opponent Board Found for opponentPlayerId: {opponentPlayerId}");
            return null;
        }

        return board.Id;

    }

    //public async Task<string> CheckShot(int boardId, int rowNumber, char cellValue)
    //{
    //    //TODO validate if shot hit a ship on board
    //    //var allShips = _shipRepository.GetAll();
    //    //var ships = allShips.Select(x => x.BoardId == boardId);

    //    return 
    //}
}
