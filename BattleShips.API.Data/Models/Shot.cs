using System.ComponentModel.DataAnnotations;
using BattleShips.API.Data.Access;

namespace BattleShips.API.Data.Models;

public class Shot : IEntity
{
    public int BoardId { get; init; }
    public int X { get; set; }
    public int Y { get; set; }

    [MaxLength(20)] public string ShotStatus { get; init; } = string.Empty;

    [Key] public int Id { get; set; }
}