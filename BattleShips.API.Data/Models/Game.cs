﻿using System.ComponentModel.DataAnnotations;
using BattleShips.API.Data.Access;

namespace BattleShips.API.Data.Models;

public class Game : IEntity
{
    public string GameCode { get; init; } = string.Empty;
    public DateTime DateCreated { get; set; }
    public DateTime DateStarted { get; set; }
    public int Player1Id { get; init; }
    public int? Player2Id { get; set; }
    public int? PlayerWinnerId { get; set; }

    [Key] public int Id { get; set; }
}