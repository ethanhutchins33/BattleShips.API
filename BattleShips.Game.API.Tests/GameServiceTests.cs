using BattleShips.Game.API.Controllers;
using NUnit.Framework;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;

namespace BattleShips.Game.API.Tests
{
    internal class GameServiceTests
    {

        private IGameService _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new GameService();
        }

        [Test]
        public void GameService_is_not_null()
        {

            
        }
    }
}
