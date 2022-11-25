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

    public async void SetupNewGame(Player player)
    {
        var newGame = await CreateGame(player);
        await AddBoard(player, newGame);
    }

    public async Task<Game?> AddPlayerToGame(Player player, int gameId)
    {
        var game = await _gameRepository.Get(gameId);

        if (game != null)
        {
            game.Player2 = player;
            await _gameRepository.Update(game);
            return game;
        }
        else
        {
            return null;
        }

    }

    public async Task<Game> CreateGame(Player player)
    {
        var result = await _gameRepository.Add(new Game
            {
                DateCreated = DateTime.Now,
                Player1 = new Player { UserName = player.UserName }
            });
        
        return result ?? throw new InvalidOperationException();

    }

    public async Task<Board> AddBoard(Player player, Game game)
    {
        var result = await _boardRepository.Add(new Board
        {
            Player = player,
            Game = game,
        });
        return result ?? throw new InvalidOperationException();

    }

    public async Task AddShipToBoard(Ship ship, int gameId, int playerId)
    {
        await _shipRepository.Add(ship);

        var boards = _boardRepository.GetAll();

        var board = boards?.FirstOrDefault(x => x.GameId == gameId && x.PlayerId == playerId);

        if (board != null)
        {
            ship.BoardId = board.Id;
        }

        await _shipRepository.Update(ship);
    }
}
