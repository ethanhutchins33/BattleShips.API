using BattleShips.API.Data.Models;

namespace BattleShips.API.Library.Responses;
public class GetGameStateResponseDto
{
    public string GameCode { get; set; } = String.Empty;
    public Shot LastShot { get; set; } = new();
}
