using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BattleShips.API.Data.Access;

namespace BattleShips.API.Data.Models;
public class Board : IEntity
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
