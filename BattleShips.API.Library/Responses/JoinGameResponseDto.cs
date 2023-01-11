namespace BattleShips.API.Library.Responses;

public class JoinGameResponseDto
{
    public int GameId { get; set; }
    public string GameCode { get; set; } = string.Empty;
    public int PlayerId { get; set; }
    public int BoardId { get; set; }
    public int? OpponentPlayerId { get; set; }
    public int? OpponentBoardId { get; set; }
}
