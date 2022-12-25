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

    public Player? Get(Guid azureId)
    {
        var players = _playerRepository.GetAll();

        var player = players?.FirstOrDefault(x => x.AzureId == azureId);

        return player ?? null;
    }

    public async Task<Player?> Get(int id)
    {
        return await _playerRepository.Get(id);
    }

    public async Task<Player?> Add(Guid azureId)
    {
        return await _playerRepository.Add(new Player
        {
            AzureId = azureId,
        });
    }

    public async Task<Player?> Remove(int id)
    {
        return await _playerRepository.Delete(id);
    }
}
