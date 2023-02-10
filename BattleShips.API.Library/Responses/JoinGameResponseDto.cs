namespace BattleShips.API.Library.Responses;

public class JoinGameResponseDto
{
    public int GameId { get; set; }
    public int PlayerId { get; set; }
    public int BoardId { get; set; }
    public string PlayerName { get; set; } = string.Empty;
}