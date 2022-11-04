using BattleShips.Game.API.Library;
using NUnit.Framework;
using FakeItEasy;
using BattleShips.Game.API.Library.Interfaces;

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

        //[Test]
        //public void GameService_is_not_null()
        //{

            
        //}

        //[Test]
        //public void GameService_creates

        //[Test]
        //public void GetHitOrMissed_returns_hit_or_missed()
        //{
        //    var result = _sut.GetHitOrMissed(gameId, playerId, X, Y);

            

        //}
    }
}
