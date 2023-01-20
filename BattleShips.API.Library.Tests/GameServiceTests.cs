using NUnit.Framework;
using BattleShips.API.Data.Access;
using BattleShips.API.Data.Models;
using BattleShips.API.Library;
using FakeItEasy;

namespace BattleShips.API.Library.Tests;

public class GameServiceTests
{
    private IRepository<Game> _gameRepository;
    private IRepository<Board> _boardRepository;
    private IRepository<Player> _playerRepository;
    private IRepository<Ship> _shipRepository;
    private IRepository<ShipType> _shipTypeRepository;
    private IRepository<Shot> _shotRepository;
    private GameService _sut;

    [SetUp]
    public void Setup()
    {
        _gameRepository = A.Fake<IRepository<Game>>();
        _boardRepository = A.Fake<IRepository<Board>>();
        _playerRepository = A.Fake<IRepository<Player>>();
        _shipRepository = A.Fake<IRepository<Ship>>();
        _shipTypeRepository = A.Fake<IRepository<ShipType>>();
        _shotRepository = A.Fake<IRepository<Shot>>();

        _sut = new GameService(
            _gameRepository, 
            _boardRepository, 
            _playerRepository, 
            _shipRepository, 
            _shipTypeRepository, 
            _shotRepository);
    }

    [Test]
    public void SetupNewGameAsync_returns_game_with_correct_game_details()
    {
        var fakePlayer = new Player
        {
            Id = 1
        };

        var fakeGame = new Game()
        {
            Id = 1,
            GameCode = "abcd1234",
            Player1Id = fakePlayer.Id,
        };

        A.CallTo(() => _playerRepository.GetAsync(An<int>._)).Returns(fakePlayer);
        A.CallTo(() => _gameRepository.AddAsync(A<Game>._)).Returns(fakeGame);
        var result = _sut.SetupNewGameAsync(fakePlayer.Id);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result.Player1Id, Is.EqualTo(fakePlayer.Id));
        Assert.That(result.Result.GameCode, Is.EqualTo(fakeGame.GameCode));
    }

    [Test]
    public void AddPlayerToGameAsync_returns_correct_player1_if_already_joined()
    {
        var testPlayer = new Player()
        {
            Id = 1,
        };

        var testGame = new Game
        {
            Id = 1,
            DateCreated = DateTime.Now,
            Player1Id = 1,
        };

        A.CallTo(() => _gameRepository.GetAsync(An<int>._)).Returns(testGame);
        A.CallTo(() => _playerRepository.GetAsync(An<int>._)).Returns(testPlayer);

        var result = _sut.AddPlayerToGameAsync(testPlayer.Id, testGame.Id);

        Assert.That(result.Result.Player1Id, Is.EqualTo(testPlayer.Id));
    }

    [Test]
    public void AddPlayerToGameAsync_returns_correct_player2_if_not_joined()
    {
        var testPlayer = new Player()
        {
            Id = 2,
        };

        var testGame = new Game
        {
            Id = 1,
            DateCreated = DateTime.Now,
            Player1Id = 1,
        };

        A.CallTo(() => _gameRepository.GetAsync(An<int>._)).Returns(testGame);
        A.CallTo(() => _playerRepository.GetAsync(An<int>._)).Returns(testPlayer);

        var result = _sut.AddPlayerToGameAsync(testPlayer.Id, testGame.Id);

        Assert.That(result.Result.Player2Id, Is.EqualTo(testPlayer.Id));
    }

    [Test]
    public void AddPlayerToGameAsync_wont_accept_third_joining_player_if_2_players_already_joined()
    {
        var testPlayer = new Player()
        {
            Id = 3,
        };

        var testGame = new Game
        {
            Id = 1,
            DateCreated = DateTime.Now,
            Player1Id = 1,
            Player2Id = 2,
        };

        A.CallTo(() => _gameRepository.GetAsync(An<int>._)).Returns(testGame);
        A.CallTo(() => _playerRepository.GetAsync(An<int>._)).Returns(testPlayer);

        var result = _sut.AddPlayerToGameAsync(testPlayer.Id, testGame.Id);

        Assert.That(result.Result.Player1Id, Is.Not.EqualTo(testPlayer.Id));
        Assert.That(result.Result.Player2Id, Is.Not.EqualTo(testPlayer.Id));
    }

    [Test]
    public void GetGameByGameCode_returns_correct_game()
    {
        var testGame1 = new Game
        {
            Id = 1,
            GameCode = "testCode1",
        };

        var testGame2 = new Game
        {
            Id = 2,
            GameCode = "testCode2",
        };

        var listGames = new List<Game> { testGame1, testGame2 };

        A.CallTo(() => _gameRepository.GetAll()).Returns(listGames.AsQueryable());

        var result = _sut.GetGameByGameCode(testGame1.GameCode);

        Assert.That(result.GameCode, Is.EqualTo(testGame1.GameCode));
        Assert.That(result.GameCode, Is.Not.EqualTo(testGame2.GameCode));
    }

    [Test]
    public void NewBoardAsync_returns_current_board_if_it_already_exists()
    {

        const int testPlayerId = 1;
        const int testGameId = 1;

        var expectedGetAllBoardsResult = new List<Board>
        {
            new Board
            {
                Id = 1,
                GameId = 1,
                IsReady = false,
                PlayerId = 1,
            },
            new Board()
            {
                Id = 2,
                GameId = 1,
                IsReady = false,
                PlayerId = 2,
            }
        };

        A.CallTo(() => _boardRepository.GetAll()).Returns(expectedGetAllBoardsResult.AsQueryable());

        var result = _sut.NewBoardAsync(testPlayerId, testGameId);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.TypeOf<Board>());
        Assert.That(result.Result.GameId, Is.EqualTo(testGameId));
    }

    [Test]
    public void NewBoardAsync_returns_new_board_if_it_does_NOT_exist()
    {

        var testPlayerId = 1;
        var testGameId = 1;

        var expectedGetAllBoardsResult = new List<Board>();

        var expectedBoard = new Board()
        {
            PlayerId = testPlayerId,
            GameId = testGameId,
        };

        A.CallTo(() => _boardRepository.GetAll()).Returns(expectedGetAllBoardsResult.AsQueryable());

        A.CallTo(() => _boardRepository.AddAsync(A<Board>._)).Returns(expectedBoard);

        var result = _sut.NewBoardAsync(testPlayerId, testGameId);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.TypeOf<Board>());
        Assert.That(result.Result.GameId, Is.EqualTo(testGameId));
    }

    [Test]
    public void AddShipsToBoardAsync_adds_correct_number_of_ships_to_db()
    {
        const int testPlayerId = 1;
        const string testGameCode = "test1234";

        string[,] testShips =
        {
            { "S", "S", "S", "", "", "", "" },
            { "", "", "", "", "", "", "" },
            { "", "", "", "", "", "", "" },
            { "", "", "", "", "", "", "" },
            { "", "", "", "", "", "", "" },
            { "", "", "", "", "", "", "" },
            { "", "", "", "", "S", "S", "S" },
        };

        var expectedGetAllGameList = new List<Game>
        {
            new()
            {
                Id = 1,
                GameCode = testGameCode,
            },
        };

        var expectedGetAllBoardList = new List<Board>
        {
            new() 
            {
                Id = 1,
                PlayerId = testPlayerId,
                GameId = 1,
            }
        };

        A.CallTo(() => _gameRepository.GetAll()).Returns(expectedGetAllGameList.AsQueryable());
        A.CallTo(() => _boardRepository.GetAll()).Returns(expectedGetAllBoardList.AsQueryable());

        _ = _sut.AddShipsToBoardAsync(testShips, testGameCode, testPlayerId);

        A.CallTo(() => _shipRepository.AddAsync(A<Ship>._))
            .MustHaveHappenedANumberOfTimesMatching((n) => n == 6);
    }

    [Test]
    public void GetBoard_returns_correct_board()
    {
        var board1 = new Board { 
            GameId = 1, 
            IsReady = false, 
            PlayerId = 1
        };

        var board2 = new Board
        {
            GameId = 1,
            IsReady = false,
            PlayerId = 2
        };

        var listBoards = new List<Board> { board1, board2 };

        A.CallTo(() => _boardRepository.GetAll()).Returns(listBoards.AsQueryable());

        var result = _sut.GetBoard(1, 1);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.GameId, Is.EqualTo(board1.GameId));
        Assert.That(result.PlayerId, Is.EqualTo(board1.PlayerId));
    }
}