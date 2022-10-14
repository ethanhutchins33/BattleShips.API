using Microsoft.AspNetCore.Mvc;

namespace BattleShips.Game.API.Controllers
{
    public class GameController : Controller
    {
        public GameController(IGameService GameService)
        {

        }

        public IActionResult CreateGame()
        {

            return null;
        }
    }
}
