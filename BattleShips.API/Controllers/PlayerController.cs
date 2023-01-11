using BattleShips.API.Data.Models;
using BattleShips.API.Library;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace BattleShips.API.Controllers;

[Authorize]
[ApiController]
[Route("api/player")]
public class PlayerController : ControllerBase
{
    private readonly IPlayerService _playerService;
    private static readonly string[] Scope = { "battleships.api" };

    public PlayerController(IPlayerService playerService)
    {
        _playerService = playerService;
    }

    [HttpGet]
    [Route("get")]
    public async Task<ActionResult<Player>> Get()
    {
        HttpContext.VerifyUserHasAnyAcceptedScope(Scope);
        var azureId = HttpContext.User.Claims.Single(c => c.Type == "sub").Value;
        var profileToReturn = _playerService.Get(Guid.Parse(azureId));

        if (profileToReturn != null)
        {
            return Ok(profileToReturn);
        }

        profileToReturn = await _playerService.Add(Guid.Parse(azureId));

        return Ok(profileToReturn);
    }
}