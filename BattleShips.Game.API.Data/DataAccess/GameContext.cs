using Microsoft.EntityFrameworkCore;
using BattleShips.Game.API.Data.Models;

namespace BattleShips.Game.API.Data.DataAccess;
public class GameContext : DbContext
{
    public GameContext(DbContextOptions<GameContext> options) : base(options) { }

    public DbSet<Models.Game> Games { get; set; }
    public DbSet<Board> Boards { get; set; }
    public DbSet<Player> Players { get; set; }
    public DbSet<Ship> Ships { get; set; }
    public DbSet<ShipType> ShipTypes { get; set; }
}
