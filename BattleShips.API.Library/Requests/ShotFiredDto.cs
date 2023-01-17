using System.ComponentModel.DataAnnotations;

namespace BattleShips.API.Library.Requests;

public class ShotFiredRequestDto
{
    [Required]
    public int BoardId { get; set; }
    [Required]
    [Range(0, 6)]
    public int X { get; set; }
    [Required]
    [Range(0, 6)]
    public int Y { get; set; }
}