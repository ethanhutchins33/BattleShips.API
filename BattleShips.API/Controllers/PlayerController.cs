using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using BattleShips.API.Data.Access;
using BattleShips.API.Data.Models;
using BattleShips.API.Library;

namespace BattleShips.API.Controllers;

[Authorize]
[ApiController]
[Route("api/player")]
public class PlayerController : ControllerBase
{
    private readonly IPlayerService _playerService;
    private static readonly string[] Scope = { "users.api" };

    public PlayerController(IPlayerService playerService)
    {
        _playerService = playerService;
    }

    [HttpGet]
    [Route("get")]
    public ActionResult<Player> Get()
    {
        HttpContext.VerifyUserHasAnyAcceptedScope(Scope);
        var azureId = HttpContext.User.Claims.Single(c => c.Type == "sub").Value;
        var profileToReturn = _playerService.Get(Guid.Parse(azureId));

        return Ok(profileToReturn);
    }

    [HttpPost]
    [Route("add")]
    public async Task<ActionResult<Player>> Add()
    {
        HttpContext.VerifyUserHasAnyAcceptedScope(Scope);

        var newId = HttpContext.User.Claims.Single(c => c.Type == "sub").Value;
        var player = _playerService.Get(Guid.Parse(newId));

        if (player != null)
        {
            return Ok(player);
        }

        player = await _playerService.Add(Guid.Parse(newId));

        return Ok(_playerService.Get(Guid.Parse(newId)));
    }

    //add friend

    //remove friend
}