using System.ComponentModel.DataAnnotations;
using BattleShips.API.Data.Access;

namespace BattleShips.API.Data.Models;

public class Board : IEntity
{
    public int PlayerId { get; init; }
    public int GameId { get; init; }
    public bool IsReady { get; set; } = false;

    [Key] public int Id { get; set; }
}