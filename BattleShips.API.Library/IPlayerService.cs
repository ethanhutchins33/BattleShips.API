using BattleShips.API.Data.Models;

namespace BattleShips.API.Library;

public interface IPlayerService
{
    Player? Get(Guid azureId);
    Task<Player?> Get(int id);
    Task<Player?> Add(Guid azureId);
    Task<Player?> Remove(int id);
}