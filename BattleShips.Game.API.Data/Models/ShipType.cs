﻿using System.ComponentModel.DataAnnotations;

namespace BattleShips.Game.API.Data.Models;
public class ShipType
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public byte Size { get; set; }
}
