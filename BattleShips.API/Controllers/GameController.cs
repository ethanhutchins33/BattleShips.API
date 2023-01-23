using Microsoft.AspNetCore.Mvc;
using BattleShips.API.Library;
using BattleShips.API.Library.Requests;
using BattleShips.API.Library.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;

namespace BattleShips.API.Controllers;

[Authorize]
[EnableCors("AllowSpecificOriginPolicy")]
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
        var azureId = HttpContext.User.Claims.Single(c => c.Type == "sub").Value;

        var player = _playerService.Get(Guid.Parse(azureId));

        if(player == null)
        {
            return BadRequest($"{nameof(CreateGame)}: Could not find player with Azure Id: {azureId}");
        }

        var newGame = await _gameService.SetupNewGameAsync(player.Id);

        if (newGame == null)
        {
            return StatusCode(500, $"{nameof(CreateGame)}: Could not setup new game with Player Id: {player.Id}");
        }

        return Ok(
            new CreateGameResponseDto
            {
                GameId = newGame.Id,
                GameCode = newGame.GameCode
            });
    }

    [HttpPost]
    [Route("join/{gameCode}")]
    public async Task<ActionResult<JoinGameResponseDto>> JoinGame(string gameCode)
    {
        var azureId = HttpContext.User.Claims.Single(c => c.Type == "sub").Value;

        var player = _playerService.Get(Guid.Parse(azureId));
        if (player == null)
        {
            return StatusCode(500, $"{nameof(JoinGame)}: No Player found with Azure ID: {azureId}");
        }

        var game = _gameService.GetGameByGameCode(gameCode);
        if (game == null)
        {
            return StatusCode(500, $"{nameof(JoinGame)}: No Game found with Game Code: {gameCode}");
        }

        var gameToReturn = await _gameService.AddPlayerToGameAsync(player.Id, game.Id);
        if (gameToReturn == null)
        {
            return StatusCode(500, $"{nameof(JoinGame)}: No Game found when trying to add player to Game Id: {game.Id} with PlayerId: {player.Id}");
        }

        //check if board already exists
        var boardToReturn = _gameService.GetBoard(game.Id, player.Id) ?? 
                            await _gameService.NewBoardAsync(player.Id, game.Id);

        if (boardToReturn == null)
        {
            return StatusCode(500, $"{nameof(JoinGame)}: No Board created when trying to add player to Game Id: {game.Id} with PlayerId: {player.Id}");
        }

        return Ok(new JoinGameResponseDto
        {
            GameId = gameToReturn.Id,
            BoardId = boardToReturn.Id,
            PlayerId = player.Id,
            PlayerName = player.UserName,
        });

    }

    [HttpPost]
    [Route("addships")]
    public async Task<ActionResult<AddShipsResponseDto>> AddPlayerShips(AddShipsRequestDto addShips)
    {
        await _gameService.AddShipsToBoardAsync(addShips.Board, addShips.GameCode, addShips.PlayerId);

        await _gameService.ReadyUpAsync(addShips.GameCode, addShips.PlayerId);

        return Ok(
            new AddShipsResponseDto
            {
                GameCode = addShips.GameCode,
                PlayerId = addShips.PlayerId
            });
    }

    [HttpPost]
    [Route("fire")]
    public async Task<ActionResult<ShotFiredResponseDto>> ShotFired(ShotFiredRequestDto shotFiredRequestDto)
    {
        Console.WriteLine(shotFiredRequestDto.BoardId);
        Console.WriteLine($"Shot: ({shotFiredRequestDto.X}, {shotFiredRequestDto.Y})");

        var result = await _gameService.CheckShotAsync
            (
                shotFiredRequestDto.BoardId,
                shotFiredRequestDto.X,
                shotFiredRequestDto.Y
            );

        if (result == null)
        {
            return NoContent();
        }

        return Ok(new ShotFiredResponseDto
        {
            ShotResult = result.ShotStatus
        });
    }

    [HttpGet]
    [Route("ready/{gameId}")]
    public async Task<ActionResult<GetLobbyDetailsResponseDto>> PollLobbyDetails(int gameId)
    {
        var playersReady = await _gameService.GetLobbyReadyStatusAsync(gameId);

        return Ok(new GetLobbyDetailsResponseDto
        {
            LobbyStatus = playersReady
        });
    }

    [HttpPost]
    [Route("start/{gameId:int}")]
    public async Task<ActionResult<StartResponseDto>> StartGame(int gameId)
    {
        var dateGameStarted = await _gameService.SetGameStartedDateTimeAsync(gameId);

        return Ok(new StartResponseDto
        {
            DateGameStarted = dateGameStarted
        });
    }

    //[HttpGet]
    //[Route("first/{gameId}")]
    //public async Task<ActionResult<GetPlayerTurnResponseDto>> GetPlayerTurn(int gameId)
    //{
    //    var firstPlayer = await _gameService.GetPlayerTurnBoardId(gameId);

    //    return Ok(new GetPlayerTurnResponseDto
    //    {

    //    });
    //}

    [HttpGet]
    [Route("state/{gameId}/{hostId}")]
    public async Task<ActionResult<GetFullGameStateResponseDto>> GetFullGameState(int gameId, int hostId)
    {
        var host = await _playerService.GetAsync(hostId);

        if (host == null)
        {
            return StatusCode(500, $"{nameof(JoinGame)}: No Player found with Player Id: {hostId}");
        }

        var hostBoard = _gameService.GetBoard(gameId, host.Id);

        if (hostBoard == null)
        {
            return StatusCode(500, $"{nameof(JoinGame)}: No Board found for Player Id: {hostId}");
        }

        var hostShipsMatrix = _gameService.GetShipsMatrix(hostBoard.Id);

        if (hostShipsMatrix.ToString() == null)
        {
            return StatusCode(500, $"{nameof(JoinGame)}: No Ships found for Board Id: {hostBoard.Id}");
        }

        var opponent =
            await _gameService.GetOpponentAsync(gameId, hostId);

        if (opponent == null)
        {
            return Ok(new GetFullGameStateResponseDto
            {
                GameId = gameId,
                HostId = host.Id,
                HostName = host.UserName,
                HostBoardId = hostBoard.Id,
                HostBoard = hostShipsMatrix,
            });
        }

        var opponentBoard = _gameService.GetBoard(gameId, opponent.Id);

        if (opponentBoard == null)
        {
            return Ok(new GetFullGameStateResponseDto
            {
                GameId = gameId,
                HostId = host.Id,
                HostName = host.UserName,
                HostBoardId = hostBoard.Id,
                HostBoard = hostShipsMatrix,
            });
        }

        return Ok(new GetFullGameStateResponseDto
        {
            GameId = gameId,
            HostId = host.Id,
            HostName = host.UserName,
            HostBoardId = hostBoard.Id,
            HostBoard = hostShipsMatrix,
            HostReadyStatus = hostBoard.IsReady,
            OpponentId = opponent.Id,
            OpponentName = opponent.UserName,
            OpponentBoardId = opponentBoard.Id,
            OpponentReadyStatus = opponentBoard.IsReady,
        });
    }

    [HttpGet]
    [Route("last/{gameId}")]
    public ActionResult<GetGameStateResponseDto> GetGameState(int gameId)
    {
        var lastShot = _gameService.GetLastShot(gameId);

        if (lastShot == null)
        {
            return NoContent();
        }

        return Ok(new GetGameStateResponseDto {
            LastShot = lastShot,
        });
    }
}