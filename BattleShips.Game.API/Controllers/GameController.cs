using Microsoft.AspNetCore.Mvc;

namespace BattleShips.Game.API.Controllers
{
    public class GameController : Controller
    {
        private readonly IGameService _gameService;

        public GameController(IGameService GS)
        {
            _gameService = GS;
        }

        public IActionResult CreateGame()
        {
            var gameId = _gameService.CreateNewGameId();
            return Ok(gameId);
        }
    }
}
