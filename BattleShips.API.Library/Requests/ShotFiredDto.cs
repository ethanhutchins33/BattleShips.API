namespace BattleShips.API.Library.Requests;

public class ShotFiredDto
{
    public string GameCode { get; set; }
    public int BoardId { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
}