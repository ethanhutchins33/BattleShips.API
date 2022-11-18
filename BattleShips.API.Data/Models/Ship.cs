using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BattleShips.API.Data.Access;

namespace BattleShips.API.Data.Models;
public class Ship : IEntity
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("BoardId")]
    public Board Board { get; set; }
    public int BoardId { get; set; }

    [ForeignKey("ShipTypeId")]
    public ShipType ShipType { get; set; }
    public int ShipTypeId { get; set; }

    public byte PosX { get; set; }
    public byte PosY { get; set; }
    public bool IsVertical { get; set; }
}
