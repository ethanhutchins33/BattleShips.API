using Microsoft.AspNetCore.Mvc;
using BattleShips.API.Library;
using BattleShips.API.Library.Requests;
using BattleShips.API.Library.Responses;
using Microsoft.AspNetCore.Authorization;

namespace BattleShips.API.Controllers;

[Authorize]
[ApiController]
[Route("api/game")]
public class GameController : ControllerBase
{
    private readonly IGameService _gameService;
    private readonly IPlayerService _playerService;

    public GameController(IGameService gameService, IPlayerService playerService)
    {
        _gameService = gameService;
        _playerService = playerService;
    }

    [HttpPost]
    [Route("create")]
    public async Task<ActionResult<CreateGameResponseDto>> CreateGame()
    {
        //TODO validate dto
        var azureId = HttpContext.User.Claims.Single(c => c.Type == "sub").Value;
        var newGame = await _gameService.SetupNewGame(_playerService.Get(Guid.Parse(azureId)).Id);

        if (newGame != null)
        {
            return Ok(new CreateGameResponseDto { GameId = newGame.Id, GameCode = newGame.GameCode });
        }
        return BadRequest();
    }


    //TODO JOIN GAME
    [HttpPost]
    [Route("join/{gameCode}")]
    public async Task<ActionResult<JoinGameResponseDto>> JoinGame(string gameCode)
    {
        var azureId = HttpContext.User.Claims.Single(c => c.Type == "sub").Value;
        var player = _playerService.Get(Guid.Parse(azureId));
        var game = _gameService.GetGameByGameCode(gameCode);

        var gameToReturn = await _gameService.AddPlayerToGame(_playerService.Get(Guid.Parse(azureId)).Id, game.Id);

        var gameToReturn = await _gameService.AddPlayerToGame(player.Id, game.Id);
        var boardToReturn = await _gameService.AddBoard(player.Id, game.Id);

        var opponentId = await _gameService.GetOpponentId(player.Id, game.Id);
        var opponentBoardId = _gameService.GetOpponentBoardId(opponentId, game.Id);

        if (gameToReturn == null)
        {
            return NoContent();
        }

        if (boardToReturn == null)
        {
            return NoContent();
        }

        return Ok(new JoinGameResponseDto
        {
            GameId = gameToReturn.Id,
            GameCode = gameCode,
            BoardId = boardToReturn.Id,
            PlayerId = player.Id,
            OpponentPlayerId = opponentId,
            OpponentBoardId = opponentBoardId,
        });

    }

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