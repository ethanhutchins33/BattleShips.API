using BattleShips.API.Data.Models;

namespace BattleShips.API.Data.Access.Repositories;

public class ShotRepository : Repository<Shot, BattleShipsContext>
{
    public ShotRepository(BattleShipsContext context) : base(context)
    {
    }
}