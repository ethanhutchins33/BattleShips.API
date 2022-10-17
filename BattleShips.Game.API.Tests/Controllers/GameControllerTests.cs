using BattleShips.Game.API.Controllers;
using NUnit.Framework;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;

namespace BattleShips.Game.API.Tests.Controllers
{
    internal class GameControllerTests
    {
        private IGameService _fakeGameService = A.Fake<IGameService>();
        private GameController _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new GameController(_fakeGameService);
        }
        
        [Test]
        public void CreateGame_should_return_OkResult()
        {
            var response = _sut.CreateGame();
            Assert.That(response, Is.TypeOf<OkResult>());
        }

        [Test]
        public void CreateGame_should_create_game()
        {
            //should return a game URL/Id for sharing
            _ = _sut.CreateGame();
            A.CallTo(() => _fakeGameService.CreateNewGame()).MustHaveHappened();
        }

        [Test]
        public void CreateGame_should_return_game_Id()
        {
            var response = _sut.CreateGame();
            

            
        }
    }
}
