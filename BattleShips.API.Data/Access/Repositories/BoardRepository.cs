using BattleShips.API.Data.Models;

namespace BattleShips.API.Data.Access.Repositories;

public class BoardRepository : Repository<Board, BattleShipsContext>
{
    public BoardRepository(BattleShipsContext context) : base(context)
    {
    }
}