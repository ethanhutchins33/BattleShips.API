using BattleShips.API.Library.Interfaces;

namespace BattleShips.API.Library;

public class GameService : IGameService
{
    public string CreateNewGameId()
    {
        return "NewGame1";
    }

    public string GetShotResult(int X, int Y, string userId)
    {
        //Check DB to find cells of userId and check if the shot hit or missed.
        return "hit";
    }
}
