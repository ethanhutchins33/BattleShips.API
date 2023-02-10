using System.ComponentModel.DataAnnotations;
using BattleShips.API.Data.Access;

namespace BattleShips.API.Data.Models;

public class Shot : IEntity
{
    public int BoardId { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public string ShotStatus { get; set; } = string.Empty;

    [Key] public int Id { get; set; }
}