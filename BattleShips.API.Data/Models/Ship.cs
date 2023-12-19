using System.ComponentModel.DataAnnotations;
using BattleShips.API.Data.Access;

namespace BattleShips.API.Data.Models;

public class Ship : IEntity
{
    public int BoardId { get; init; }
    public int ShipTypeId { get; set; }
    public int PosX { get; init; }
    public int PosY { get; init; }
    public bool IsVertical { get; set; }

    [Key] public int Id { get; set; }
}