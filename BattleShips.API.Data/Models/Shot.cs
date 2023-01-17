using BattleShips.API.Data.Access;
using System.ComponentModel.DataAnnotations;

namespace BattleShips.API.Data.Models;
public class Shot : IEntity
{
    [Key]
    public int Id { get; set; }
    public int BoardId { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public string ShotStatus = string.Empty;
}
