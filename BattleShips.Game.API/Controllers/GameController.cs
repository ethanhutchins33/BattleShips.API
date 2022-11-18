using Microsoft.AspNetCore.Mvc;
using BattleShips.Game.API.Data.DataAccess;
using BattleShips.Game.API.Data.Models;

namespace BattleShips.Game.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameController : ControllerBase
{
    private readonly IGameRepository _gameRepository;

    public GameController(IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }

    [HttpGet("id")]
    public async Task<ActionResult<Data.Models.Game>> GetGame(int id)
    {
        return Ok(await _gameRepository.GetGameAsync(id));
    }

    [HttpPost]
    public async Task<ActionResult<Data.Models.Game>> CreateGame(Player player)
    {
        await _gameRepository.CreateGameAsync(new Data.Models.Game
        {
            DateCreated = DateTime.Now,
            Player1 = new Player{UserName = player.UserName},
        });

        return Ok();
    }

    [HttpPut("id")]
    public async Task<IActionResult> UpdateAuthor(int id, Data.Models.Game game)
    {
        if (id != game.Id)
        {
            return BadRequest();
        }

        Data.Models.Game dbGame = await _gameRepository.UpdateGameAsync(game);

        if (dbGame == null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Game with Id: {game.Id} could not be updated");
        }

        return NoContent();
    }

}