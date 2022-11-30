using System.ComponentModel.DataAnnotations;
using BattleShips.API.Data.Access;

namespace BattleShips.API.Data.Models;

public class Game : IEntity
{
    [Key]
    public int Id { get; set; }
    public DateTime DateCreated { get; set; }
    public int? Player1Id { get; set; }
    public int? Player2Id { get; set; }
    public int? PlayerWinnerId { get; set; }
}