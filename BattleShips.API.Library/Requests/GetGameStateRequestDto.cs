using System.ComponentModel.DataAnnotations;

namespace BattleShips.API.Library.Requests;
public class GetGameStateRequestDto
{
    [Required]
    [StringLength(8)]
    public string GameCode { get; set; } = String.Empty;
    [Required]
    public int PlayerId { get; set; }
}
