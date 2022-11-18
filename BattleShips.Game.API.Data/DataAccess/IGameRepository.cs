
namespace BattleShips.Game.API.Data.DataAccess;
public interface IGameRepository
{
    public void CreateGame();
    public Models.Game ReturnGame(int gameId);
    public void UpdateGame();
    public void DeleteGame();
}
