using Microsoft.AspNetCore.Mvc;
using BattleShips.API.Data.Models;
using BattleShips.API.Library;

namespace BattleShips.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameController : ControllerBase
{
    private readonly IGameService _gameService;

    public GameController(IGameService gameService)
    {
        _gameService = gameService;
    }

    [HttpPost]
    public async Task<ActionResult<Game>> CreateGame(Player player)
    {
        var result = await _gameService.CreateGame(player);

        return Ok(result);
    }

    //[HttpGet("id")]
    //public async Task<ActionResult<Game>> GetGame(int id)
    //{


    //    return Ok();
    //}

    //[HttpPut("id")]
    //public async Task<IActionResult> UpdateGame(int id, Game? game)
    //{
    //    if (game != null && id != game.Id)
    //    {
    //        return BadRequest();
    //    }

    //    Game? dbGame = await _repository.Update(game);

    //    if (dbGame == null)
    //    {
    //        return StatusCode(StatusCodes.Status500InternalServerError, $"Game with Id: {id} could not be updated");
    //    }

    //    return Ok(dbGame);
    //}

}