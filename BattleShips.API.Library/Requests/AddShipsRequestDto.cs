namespace BattleShips.API.Library.Requests;
public class AddShipsRequestDto
{
    public string[,] Board { get; set; } = new string[,] { { } };
    public string GameCode { get; set; } = string.Empty;
    public int PlayerId { get; set; }
}
