using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BattleShips.API.Data.Access;

namespace BattleShips.API.Data.Models;

public class Game : IEntity
{
    [Key]
    public int Id { get; set; }
    public DateTime DateCreated { get; set; }

    [ForeignKey("Player1Id")]
    public Player? Player1 { get; set; }
    public int? Player1Id { get; set; }

    [ForeignKey("Player2Id")]
    public Player? Player2 { get; set; }
    public int? Player2Id { get; set; }

    [ForeignKey("PlayerWinnerId")]
    public Player? PlayerWinner { get; set; }
    public int? PlayerWinnerId { get; set; }

    public Game()
    {
        DateCreated = DateTime.Now;
        Player1 = new Player();
        Player2 = new Player();
    }
}