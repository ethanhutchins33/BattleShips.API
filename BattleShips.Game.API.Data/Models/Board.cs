namespace BattleShips.Game.API.Data.Models;
public class Board
{
    public int Id { get; set; }
    public Player PlayerId { get; set; }
    public Game GameId { get; set; }
}
