using BattleShips.API.Data.Models;

namespace BattleShips.API.Data.Access.Repositories;
public class PlayerRepository : Repository<Player, BattleShipsContext>
{
    public PlayerRepository(BattleShipsContext context) : base(context)
    {

    }
}
