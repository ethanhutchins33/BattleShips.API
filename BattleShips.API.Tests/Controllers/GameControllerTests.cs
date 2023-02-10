using BattleShips.API.Controllers;
using BattleShips.API.Library;
using BattleShips.API.Library.Responses;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace BattleShips.API.Tests.Controllers;

internal class GameControllerTests
{
    private IGameService _fakeGameService;
    private IPlayerService _fakePlayerService;
    private GameController _sut;

    [SetUp]
    public void Setup()
    {
        _fakeGameService = A.Fake<IGameService>();
        _fakePlayerService = A.Fake<IPlayerService>();

        _sut = new GameController(_fakeGameService, _fakePlayerService);
    }

    [Test]
    public void CreateGame_should_return_Dto()
    {
        var response = _sut.CreateGame();

        Assert.That(response, Is.TypeOf<Task<ActionResult<CreateGameResponseDto>>>());
    }

    //[Test]
    //public void CreateGame_should_call_to_GameService_to_create_game()
    //{
    //    _ = _sut.CreateGame();

    //    A.CallTo(() => _fakeGameService.SetupNewGameAsync(1)).MustHaveHappened();
    //}

    //[Test]
    //public void CreateGame_should_return_id_from_GameService()
    //{
    //    A.CallTo(() => _fakeGameService.SetupNewGameAsync(1))
    //        .Returns(new Game { Id = 1 });

    //    var response = _sut.CreateGame();

    //    Assert.That(response.Id, Is.EqualTo(1));
    //}
}