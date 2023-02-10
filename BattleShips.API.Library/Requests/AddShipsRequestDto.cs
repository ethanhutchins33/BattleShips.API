using System.ComponentModel.DataAnnotations;

namespace BattleShips.API.Library.Requests;

public class AddShipsRequestDto
{
    [Required] [StringLength(8)] public string GameCode { get; set; } = string.Empty;

    [Required] public int PlayerId { get; set; }

    [Required] public string[,] Board { get; set; } = { { } };
}