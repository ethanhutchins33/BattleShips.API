using BattleShips.Game.API.Library.Interfaces;
using BattleShips.Game.API.Library.Models;

namespace BattleShips.Game.API.Library;

public class GameService : IGameService
{
    public string CreateNewGameId()
    {
        return "NewGame1";
    }

    public CellState GetShotResultCellState(int X, int Y, string userId)
    {
        //Check DB to find cells of userId and check if the shot hit or missed.

        var tempState = new CellState();

        tempState.State = "hit";

        return tempState;
    }
}
