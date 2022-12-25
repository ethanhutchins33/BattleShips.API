using Microsoft.EntityFrameworkCore;
using BattleShips.API.Data.Models;

namespace BattleShips.API.Data.Access;
public class BattleShipsContext : DbContext
{
    public BattleShipsContext(DbContextOptions<BattleShipsContext> options) : base(options) { }

    public DbSet<Game>? Games { get; set; }
    public DbSet<Board>? Boards { get; set; }
    public DbSet<Player>? Players { get; set; }
    public DbSet<Ship>? Ships { get; set; }
    public DbSet<ShipType>? ShipTypes { get; set; }
}
