using System.ComponentModel.DataAnnotations;

namespace BattleShips.API.Library.Requests;
public class JoinGameDto
{
    [Required]
    public int JoiningPlayerId { get; set; }
}
