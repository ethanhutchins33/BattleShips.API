using System.Net;
using BattleShips.API.Library;
using BattleShips.API.Library.Requests;
using BattleShips.API.Library.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace BattleShips.API.Controllers;

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
    public async Task<ActionResult<CreateGameResponseDto>> CreateGame(string userName)
    {
        var player = _playerService.Get(userName);
        if (player is null)
            return BadRequest(
                $"{nameof(CreateGame)}: Could not find player with Username: {userName}");

        var newGame = await _gameService.SetupNewGameAsync(player.Id);
        if (newGame is null)
            return StatusCode(500,
                $"{nameof(CreateGame)}: Could not setup new game with Player Id: {player.Id}");

        var board = await _gameService.NewBoardAsync(player.Id, newGame.Id);

        return Ok(
            new CreateGameResponseDto
            {
                GameId = newGame.Id,
                GameCode = newGame.GameCode,
                BoardId = board.Id
            });
    }

    [HttpPost]
    [Route("join/{gameCode}")]
    public async Task<ActionResult<JoinGameResponseDto>> JoinGame(string userName, string gameCode)
    {
        var player = _playerService.Get(userName);
        if (player is null)
            return StatusCode((int)HttpStatusCode.BadRequest,
                $"{nameof(JoinGame)}: No Player found with Username: {userName}");

        var game = _gameService.GetGameByGameCode(gameCode);
        if (game is null)
            return StatusCode((int)HttpStatusCode.BadRequest,
                $"{nameof(JoinGame)}: No Game found with Game Code: {gameCode}");

        await _gameService.AddPlayerToGameAsync(player.Id, game);

        //check if board already exists
        var boardToReturn = _gameService.GetBoard(game.Id, player.Id) ??
                            await _gameService.NewBoardAsync(player.Id, game.Id);

        return Ok(new JoinGameResponseDto
        {
            GameId = game.Id,
            BoardId = boardToReturn.Id,
            PlayerId = player.Id,
            PlayerName = player.UserName
        });
    }

    [HttpPost]
    [Route("addships")]
    public async Task<ActionResult<AddShipsResponseDto>> AddPlayerShips(
        AddShipsRequestDto addShips)
    {
        await _gameService.AddShipsToBoardAsync(addShips.Board, addShips.GameCode,
            addShips.PlayerId);

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
    public async Task<ActionResult<ShotFiredResponseDto>> ShotFired(
        ShotFiredRequestDto shotFiredRequestDto)
    {
        Console.WriteLine(shotFiredRequestDto.BoardId);
        Console.WriteLine($"Shot: ({shotFiredRequestDto.X}, {shotFiredRequestDto.Y})");

        var result = await _gameService.CheckShotAsync
        (
            shotFiredRequestDto.BoardId,
            shotFiredRequestDto.X,
            shotFiredRequestDto.Y
        );

        if (result is null) return NoContent();

        return Ok(new ShotFiredResponseDto
        {
            ShotResult = result.ShotStatus
        });
    }

    [HttpGet]
    [Route("ready/{gameId}")]
    public async Task<ActionResult<GetLobbyDetailsResponseDto>> PollLobbyDetails(
        int gameId)
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
    public async Task<ActionResult<GetFullGameStateResponseDto>> GetFullGameState(
        int gameId, int hostId)
    {
        var host = await _playerService.GetAsync(hostId);

        if (host is null)
            return StatusCode(500,
                $"{nameof(JoinGame)}: No Player found with Player Id: {hostId}");

        var hostBoard = _gameService.GetBoard(gameId, host.Id);

        if (hostBoard is null)
            return StatusCode(500,
                $"{nameof(JoinGame)}: No Board found for Player Id: {hostId}");

        var hostShipsMatrix = _gameService.GetShipsMatrix(hostBoard.Id);

        if (hostShipsMatrix.ToString() is null)
            return StatusCode(500,
                $"{nameof(JoinGame)}: No Ships found for Board Id: {hostBoard.Id}");

        var opponent =
            await _gameService.GetOpponentAsync(gameId, hostId);

        if (opponent is null)
            return Ok(new GetFullGameStateResponseDto
            {
                GameId = gameId,
                HostId = host.Id,
                HostName = host.UserName,
                HostBoardId = hostBoard.Id,
                HostBoard = hostShipsMatrix
            });

        var opponentBoard = _gameService.GetBoard(gameId, opponent.Id);

        if (opponentBoard is null)
            return Ok(new GetFullGameStateResponseDto
            {
                GameId = gameId,
                HostId = host.Id,
                HostName = host.UserName,
                HostBoardId = hostBoard.Id,
                HostBoard = hostShipsMatrix
            });

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
            OpponentReadyStatus = opponentBoard.IsReady
        });
    }

    [HttpGet]
    [Route("last/{gameId}")]
    public ActionResult<GetGameStateResponseDto> GetGameState(int gameId)
    {
        var lastShot = _gameService.GetLastShot(gameId);

        if (lastShot is null) return NoContent();

        return Ok(new GetGameStateResponseDto
        {
            LastShot = lastShot
        });
    }
}
