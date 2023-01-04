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
        
        var newGame = await CreateGame(player);
        
        if (newGame == null) return null;
        
        await AddBoard(player.Id, newGame.Id);

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
        await AddBoard(player.Id, gameId);

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

    private async Task<Game?> CreateGame(IEntity player)
    {
        var newGameCode = GenerateRandomCode();
        var result = await _gameRepository.Add(new Game
            {
                DateCreated = DateTime.Now,
                Player1Id = player.Id,
                GameCode = GenerateRandomCode(),
            });
        
        return result;

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
}
