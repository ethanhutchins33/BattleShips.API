using Microsoft.AspNetCore.Mvc;
using BattleShips.API.Data.DataAccess;
using BattleShips.API.Data.Models;

namespace BattleShips.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameController : ControllerBase
{
    private readonly IRepository<Game> _repository;

    public GameController(IRepository<Game> repository)
    {
        _repository = repository;
    }

    [HttpGet("id")]
    public async Task<ActionResult<Game>> GetGame(int id)
    {
        var result = await _repository.Get(id);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Game>> CreateGame(Player player)
    {
        await _repository.Add(new Game
        {
            DateCreated = DateTime.Now,
            Player1 = new Player{UserName = player.UserName},
        });

        return Ok();
    }

    [HttpPut("id")]
    public async Task<IActionResult> UpdateAuthor(int id, Game game)
    {
        if (id != game.Id)
        {
            return BadRequest();
        }

        Game dbGame = await _repository.Update(game);

        if (dbGame == null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Game with Id: {game.Id} could not be updated");
        }

        return NoContent();
    }

}