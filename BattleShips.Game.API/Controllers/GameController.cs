using Microsoft.AspNetCore.Mvc;
using BattleShips.Game.API.Data.DataAccess;
namespace BattleShips.Game.API.Controllers;

[ApiController]
[Route("api/game")]
public class GameController : ControllerBase
{
    private readonly IGameRepository _gameRepository;

    public GameController(IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }

    [HttpPost]
    [Route("/new")]
    public IActionResult CreateGame()
    {
        _gameRepository.CreateGame();

        return Ok();
    }

    [HttpGet]
    [Route("/get")]
    public ActionResult<Data.Models.Game> GetGame()
    {
        var result = _gameRepository.ReturnGame(1);

        if (result == null)
        {
            return BadRequest();
        }

        return Ok(result);
    }

}