using BattleShips.API.Data.Models;

namespace BattleShips.API.Data.Access.Repositories;

public class GameRepository : Repository<Game, BattleShipsContext>
{
    public GameRepository(BattleShipsContext context) : base(context)
    {
    }
}