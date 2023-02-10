using System.ComponentModel.DataAnnotations;
using BattleShips.API.Data.Access;

namespace BattleShips.API.Data.Models;

public class ShipType : IEntity
{
    public string? Name { get; set; }
    public byte Size { get; set; }

    [Key] public int Id { get; set; }
}