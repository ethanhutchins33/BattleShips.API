using Microsoft.AspNetCore.Mvc;

namespace BattleShips.Game.API.Controllers;

public class GameController : Controller
{
    

    public GameController()
    {
        
    }

    public IActionResult CreateGame()
    {
        
        return Ok();
    }


}