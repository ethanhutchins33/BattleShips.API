// using BattleShips.API.Controllers;
// using BattleShips.API.Data.DataAccess;
// using FakeItEasy;
// using Microsoft.AspNetCore.Mvc;
// using NUnit.Framework;

// namespace BattleShips.API.Tests.Controllers;

// internal class GameControllerTests
// {
//     private IRepository _fakeGameRepository;
//     private GameController _sut;

//     [SetUp]
//     public void Setup()
//     {
//         _fakeGameRepository = A.Fake<IRepository>();

//         _sut = new GameController(_fakeGameRepository);
//     }

//     [Test]
//     public void CreateGame_should_return_OkResult()
//     {
//         var response = _sut.CreateGame();

//         Assert.That(response, Is.TypeOf<OkObjectResult>());
//     }

//     [Test]
//     public void CreateGame_should_call_to_GameService_to_create_game()
//     {
//         _ = _sut.CreateGame();

//         A.CallTo(() => _fakeGameRepository.CreateGame()).MustHaveHappened();
//     }

//     [Test]
//     public void CreateGame_should_return_id_from_game_service()
//     {
//         A.CallTo(() => _fakeGameRepository.CreateGame()).Returns("abc");

//         var response = _sut.CreateGame() as OkObjectResult;

//         Assert.That(response.Value, Is.EqualTo("abc"));
//     }

// }