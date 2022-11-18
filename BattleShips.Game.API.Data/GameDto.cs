using BattleShips.Game.API.Data.Models;

namespace BattleShips.Game.API.Data;
public class GameDto
{
    public int Id { get; set; }
    public DateTime DateCreated { get; set; }

    public Player? Player1 { get; set; }
    public int? Player1Id { get; set; }

    public Player? Player2 { get; set; }
    public int? Player2Id { get; set; }

    public Player? PlayerWinner { get; set; }
    public int? PlayerWinnerId { get; set; }
}
