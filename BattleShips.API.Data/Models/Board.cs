using System.ComponentModel.DataAnnotations;
using BattleShips.API.Data.Access;

namespace BattleShips.API.Data.Models;

public class Board : IEntity
{
    public int PlayerId { get; set; }
    public int GameId { get; set; }
    public bool IsReady { get; set; } = false;

    [Key] public int Id { get; set; }
}