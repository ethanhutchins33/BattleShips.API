namespace BattleShips.API.Library.Responses;

public class JoinGameResponseDto
{
    public int GameId { get; set; }
    public string GameCode { get; set; }
    public int PlayerId { get; set; }
    public int BoardId { get; set; }
    public int? OpponentPlayerId { get; set; }
    public int? OpponentBoardId { get; set; }
}
