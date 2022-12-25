namespace BattleShips.API.Library.Response;
public class JoinGameResponseDto
{
    public int GameId { get; set; }
    public int? HostPlayerId { get; set; }
    public int? GuestPlayerId { get; set; }
}
