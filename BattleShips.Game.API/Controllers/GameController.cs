using Microsoft.AspNetCore.Mvc;

namespace BattleShips.Game.API.Controllers
{
    public class GameController : Controller
    {
        IGameService gameService;

        public GameController(IGameService GS)
        {
            gameService = GS;
        }

        public IActionResult CreateGame()
        {

            return Ok();
        }
    }
}
