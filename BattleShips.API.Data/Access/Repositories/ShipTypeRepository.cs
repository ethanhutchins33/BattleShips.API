using BattleShips.API.Data.Models;

namespace BattleShips.API.Data.Access.Repositories;
public class ShipTypeRepository : Repository<ShipType, BattleShipsContext>
{
    public ShipTypeRepository(BattleShipsContext context) : base(context)
    {

    }
}