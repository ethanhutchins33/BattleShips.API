using BattleShips.Game.API.Controllers;
using BattleShips.Game.API.Library.Interfaces;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace BattleShips.Game.API.Tests.Controllers;

internal class GameControllerTests
{
    private IGameService _fakeGameService;
    private GameController _sut;

    [SetUp]
    public void Setup()
    {
        _fakeGameService = A.Fake<IGameService>();

        _sut = new GameController(_fakeGameService);
    }

    [Test]
    public void CreateGame_should_return_OkResult()
    {
        var response = _sut.CreateGame();

        Assert.That(response, Is.TypeOf<OkObjectResult>());
    }

    [Test]
    public void CreateGame_should_call_to_GameService_to_create_game()
    {
        _ = _sut.CreateGame();

        A.CallTo(() => _fakeGameService.CreateNewGameId()).MustHaveHappened();
    }

    [Test]
    public void CreateGame_should_return_id_from_game_service()
    {
        A.CallTo(() => _fakeGameService.CreateNewGameId()).Returns("abc");

        var response = _sut.CreateGame() as OkObjectResult;

        Assert.That(response.Value, Is.EqualTo("abc"));
    }

}