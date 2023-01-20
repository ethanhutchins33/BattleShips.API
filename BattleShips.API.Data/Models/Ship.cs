using System.ComponentModel.DataAnnotations;
using BattleShips.API.Data.Access;

namespace BattleShips.API.Data.Models;
public class Ship : IEntity
{
    [Key]
    public int Id { get; set; }
    public int BoardId { get; set; }
    public int ShipTypeId { get; set; }
    public int PosX { get; set; }
    public int PosY { get; set; }
    public bool IsVertical { get; set; }
}
