namespace BattleShips.Game.API.Data.Models;
public class Ship
{
    public int Id { get; set; }
    public Board BoardId { get; set; }
    public ShipType TypeId { get; set; }
    public byte PosX { get; set; }
    public byte PosY { get; set; }
    public bool IsVertical { get; set; }
}
