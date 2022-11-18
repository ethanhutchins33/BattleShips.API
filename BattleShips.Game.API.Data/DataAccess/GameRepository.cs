
using BattleShips.Game.API.Data.Models;

namespace BattleShips.Game.API.Data.DataAccess;
public class GameRepository : IGameRepository
{
    private readonly GameContext _gameContext;

    public GameRepository(GameContext gameContext)
    {
        _gameContext = gameContext;
    }


    public void CreateGame()
    {
        _gameContext.Games.Add(new Models.Game
        {
            DateCreated = DateTime.Now,
            Player1 = new Player{UserName = "userA"},
            Player2 = new Player{UserName = "userB" },
            PlayerWinner = null,
        });

        _gameContext.SaveChanges();
    }

    public Models.Game ReturnGame(int gameId)
    {
        var gameToReturn = _gameContext.Games.FirstOrDefault(game => game.Id == gameId);

        return gameToReturn;
    }

    public void UpdateGame()
    {
        throw new NotImplementedException();
    }

    public void DeleteGame()
    {
        throw new NotImplementedException();
    }
}
