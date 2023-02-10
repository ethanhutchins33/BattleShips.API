using BattleShips.API.Data.Models;

namespace BattleShips.API.Library.Responses;

public class GetGameStateResponseDto
{
    public Shot LastShot { get; set; } = new();
}