using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BattleShips.Game.API.Data.Models;
public class Board
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("PlayerId")]
    public Player Player { get; set; }
    public int PlayerId { get; set; }

    [ForeignKey("GameId")]
    public Game Game { get; set; }
    public int GameId { get; set; }
}
