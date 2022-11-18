using BattleShips.API.Data.Models;

namespace BattleShips.API.Data.Access.Repositories;
public class ShipRepository : Repository<Ship, BattleShipsContext>
{
    public ShipRepository(BattleShipsContext context) : base(context)
    {

    }
}