namespace BattleShips.API.Library.Responses;

public class GetFullGameStateResponseDto
{
    public string GameCode { get; set; } = string.Empty;
    public int HostId { get; set; }
    public string HostName { get; set; } = string.Empty;
    public int HostBoardId { get; set; }
    public bool HostReadyStatus { get; set; }
    public string[,] HostBoard { get; set; } = { { }, { } };
    public int? OpponentId { get; set; }
    public string? OpponentName { get; set; } = string.Empty;
    public int? OpponentBoardId { get; set; }
    public bool? OpponentReadyStatus { get; set; }
}