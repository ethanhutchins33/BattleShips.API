namespace BattleShips.API.Library.Responses;

public class CreateGameResponseDto
{
    public int GameId { get; set; }
    public string GameCode { get; set; } = string.Empty;
}
