using System.ComponentModel.DataAnnotations;

namespace BattleShips.API.Library.Requests;
public class GetGameStateRequestDto
{
    [Required]
    public int GameId { get; set; }
    [Required]
    public int PlayerId { get; set; }
}
