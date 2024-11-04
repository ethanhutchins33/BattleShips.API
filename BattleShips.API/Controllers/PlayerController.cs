using BattleShips.API.Data.Models;
using BattleShips.API.Library;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace BattleShips.API.Controllers;

[ApiController]
[Route("api/player")]
public class PlayerController : ControllerBase
{
    private readonly IPlayerService _playerService;

    public PlayerController(IPlayerService playerService)
    {
        _playerService = playerService;
    }

    [HttpGet]
    [Route("get")]
    public async Task<ActionResult<Player>> Get()
    {
        var azureId = Guid.NewGuid();
        var userName = $"User-{azureId}";
        var profileToReturn = _playerService.Get(userName);

        if (profileToReturn != null) return Ok(profileToReturn);

        profileToReturn = await _playerService.AddAsync(azureId, userName);

        return Ok(profileToReturn);
    }
}