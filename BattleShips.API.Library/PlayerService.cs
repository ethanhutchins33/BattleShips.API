using BattleShips.API.Data.Access;
using BattleShips.API.Data.Models;

namespace BattleShips.API.Library;

public class PlayerService : IPlayerService
{
    private readonly IRepository<Player> _playerRepository;

    public PlayerService(IRepository<Player> playerRepository)
    {
        _playerRepository = playerRepository;
    }

    public Player? Get(string userName)
    {
        var players = _playerRepository.GetAll();

        var player = players?.FirstOrDefault(x => x.UserName == userName);

        return player ?? null;
    }

    public async Task<Player?> GetAsync(int id)
    {
        return await _playerRepository.GetAsync(id);
    }

    public async Task<Player?> AddAsync(Guid azureId, string userName)
    {
        return await _playerRepository.AddAsync(new Player
        {
            UserName = userName
        });
    }

    public async Task<Player?> RemoveAsync(int id)
    {
        return await _playerRepository.DeleteAsync(id);
    }
}