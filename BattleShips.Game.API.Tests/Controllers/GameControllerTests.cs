using BattleShips.Game.API.Controllers;
using NUnit.Framework;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;

namespace BattleShips.Game.API.Tests.Controllers
{
    internal class GameControllerTests
    {
        [Test]
        public void GameController_should_make_GameService_create_new_game()
        {
            var GS = A.Fake<IGameService>();
            var controller = new GameController(GS);

            var response = controller.CreateGame() as OkResult;

            Assert.That(response, Is.Not.Null);
        }
    }
}
