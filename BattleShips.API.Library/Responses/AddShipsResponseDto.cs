namespace BattleShips.API.Library.Responses;

public class AddShipsResponseDto
{
    public int PlayerId { get; set; }
    public string GameCode { get; set; } = string.Empty;
}