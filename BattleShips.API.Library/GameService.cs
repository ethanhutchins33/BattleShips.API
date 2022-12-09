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

    public async Task<Game> SetupNewGame(int playerId)
    {
        var player = await _playerRepository.Get(playerId) ?? 
                     await _playerRepository.Add(new Player
        {
            UserName = "test",
        });

        var newGame = await CreateGame(player);
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
                     await _playerRepository.Add(new Player
                     {
                         UserName = "P2UserName",
                     });

        game.Player2Id = player.Id;
        await _gameRepository.Update(game);
        await AddBoard(player.Id, gameId);
        return game;
    }

    private async Task<Game> CreateGame(Player player)
    {
        var result = await _gameRepository.Add(new Game
            {
                DateCreated = DateTime.Now,
                Player1Id = player.Id
            });
        
        return result;

    }

    public async Task<Board> AddBoard(int playerId, int gameId)
    {
        var board = GetBoard(playerId, gameId);

        if (board == null)
        {
            var result = await _boardRepository.Add(new Board
            {
                PlayerId = playerId,
                GameId = gameId,
            });
            return result;
        }

        return board;
        
    }

    public async Task AddShipToBoard(Ship ship, int gameId, int playerId)
    {
        await _shipRepository.Add(ship);

        var board = GetBoard(playerId, gameId);

        if (board != null)
        {
            ship.BoardId = board.Id;
        }

        await _shipRepository.Update(ship);
    }

    public async Task<string?> GetPlayerUserNameById(int? playerId)
    {
        var player = await _playerRepository.Get(playerId);

        if (player != null && player.UserName != null)
        {
            return player.UserName;
        }

        return null;
    }

    private string GenerateRandomCode()
    {
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var stringChars = new char[8];
        var random = new Random();

        for (int i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[random.Next(chars.Length)];
        }

        return new String(stringChars);
    }

    private Board? GetBoard(int playerId, int gameId)
    {
        var boards = _boardRepository.GetAll();

        var board = boards?.FirstOrDefault(x => x.GameId == gameId && x.PlayerId == playerId);

        if (board == null)
        {
            return null;
        }

        return board;
    }
}
