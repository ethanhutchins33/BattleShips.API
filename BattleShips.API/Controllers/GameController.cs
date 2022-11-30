using Microsoft.AspNetCore.Mvc;
using BattleShips.API.Data.Models;
using BattleShips.API.Library;
using BattleShips.API.Library.Requests;
using BattleShips.API.Library.Response;

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
    public async Task<ActionResult<CreateGameResponseDto>> CreateGame(CreateGameDto createGameDto)
    {
        //TODO validate dto

        var newGame = await _gameService.SetupNewGame(createGameDto.HostPlayerId);

        return Ok(new CreateGameResponseDto{GameId = newGame.Id});
    }


    //TODO JOIN GAME

    //TODO ADD SHIPS

    //TODO START GAME (after 5 ships in each board)

    //TODO FIRE AT SHIPS

    //TODO GETGAMESTATE => who's turn, winnerBool, list of shots so far

    //[HttpGet("id")]
    //public async Task<ActionResult<Game>> GetGame(Player id)
    //{
    //    return Ok();
    //}

    //[HttpPut("id")]
    //public async Task<IActionResult> UpdateGame(Player id, Game? game)
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