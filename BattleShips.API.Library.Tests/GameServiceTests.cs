using BattleShips.API.Data.Access;
using BattleShips.API.Data.Models;
using FakeItEasy;
using NUnit.Framework;

namespace BattleShips.API.Library.Tests;

public class GameServiceTests
{
    private readonly IRepository<Board> _boardRepository;
    private readonly IRepository<Game> _gameRepository;
    private readonly IRepository<Player> _playerRepository;
    private readonly IRepository<Ship> _shipRepository;
    private readonly IRepository<ShipType> _shipTypeRepository;
    private readonly IRepository<Shot> _shotRepository;
    private readonly GameService _sut;

    public GameServiceTests()
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

    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void SetupNewGameAsync_returns_game_with_correct_game_details()
    {
        //Arrange
        var fakePlayer = new Player
        {
            Id = 1
        };

        var fakeGame = new Game
        {
            Id = 1,
            GameCode = "abcd1234",
            Player1Id = fakePlayer.Id
        };

        A.CallTo(() => _playerRepository.GetAsync(An<int>._)).Returns(fakePlayer);
        A.CallTo(() => _gameRepository.AddAsync(A<Game>._)).Returns(fakeGame);

        //Act
        var result = _sut.SetupNewGameAsync(fakePlayer.Id);

        //Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result?.Player1Id, Is.EqualTo(fakePlayer.Id));
        Assert.That(result.Result?.GameCode, Is.EqualTo(fakeGame.GameCode));
    }

    [Test]
    public void AddPlayerToGameAsync_returns_correct_player1_if_already_joined()
    {
        //Arrange
        var testPlayer = new Player
        {
            Id = 1
        };

        var testGame = new Game
        {
            Id = 1,
            DateCreated = DateTime.Now,
            Player1Id = 1
        };

        A.CallTo(() => _gameRepository.GetAsync(An<int>._)).Returns(testGame);
        A.CallTo(() => _playerRepository.GetAsync(An<int>._)).Returns(testPlayer);

        //Act
        var result = _sut.AddPlayerToGameAsync(testPlayer.Id, testGame.Id);

        //Assert
        Assert.That(result.Result?.Player1Id, Is.EqualTo(testPlayer.Id));
    }

    [Test]
    public void AddPlayerToGameAsync_returns_correct_player2_if_not_joined()
    {
        //Arrange
        var testPlayer = new Player
        {
            Id = 2
        };

        var testGame = new Game
        {
            Id = 1,
            DateCreated = DateTime.Now,
            Player1Id = 1
        };

        A.CallTo(() => _gameRepository.GetAsync(An<int>._)).Returns(testGame);
        A.CallTo(() => _playerRepository.GetAsync(An<int>._)).Returns(testPlayer);

        //Act
        var result = _sut.AddPlayerToGameAsync(testPlayer.Id, testGame.Id);

        //Assert
        Assert.That(result.Result?.Player2Id, Is.EqualTo(testPlayer.Id));
    }

    [Test]
    public void AddPlayerToGameAsync_wont_accept_third_joining_player_if_2_players_already_joined()
    {
        //Arrange
        var testPlayer = new Player
        {
            Id = 3
        };

        var testGame = new Game
        {
            Id = 1,
            DateCreated = DateTime.Now,
            Player1Id = 1,
            Player2Id = 2
        };

        A.CallTo(() => _gameRepository.GetAsync(An<int>._)).Returns(testGame);
        A.CallTo(() => _playerRepository.GetAsync(An<int>._)).Returns(testPlayer);

        //Act
        var result = _sut.AddPlayerToGameAsync(testPlayer.Id, testGame.Id);

        //Assert
        Assert.That(result.Result?.Player1Id, Is.Not.EqualTo(testPlayer.Id));
        Assert.That(result.Result?.Player2Id, Is.Not.EqualTo(testPlayer.Id));
    }

    [Test]
    public void GetGameByGameCode_returns_correct_game()
    {
        //Arrange
        var testGame1 = new Game
        {
            Id = 1,
            GameCode = "testCode1"
        };

        var testGame2 = new Game
        {
            Id = 2,
            GameCode = "testCode2"
        };

        var listGames = new List<Game> { testGame1, testGame2 };

        A.CallTo(() => _gameRepository.GetAll()).Returns(listGames.AsQueryable());

        //Act
        var result = _sut.GetGameByGameCode(testGame1.GameCode);

        //Assert
        Assert.That(result?.GameCode, Is.EqualTo(testGame1.GameCode));
        Assert.That(result?.GameCode, Is.Not.EqualTo(testGame2.GameCode));
    }

    [Test]
    public void NewBoardAsync_returns_current_board_if_it_already_exists()
    {
        //Arrange
        const int testPlayerId = 1;
        const int testGameId = 1;

        var expectedGetAllBoardsResult = new List<Board>
        {
            new()
            {
                Id = 1,
                GameId = 1,
                IsReady = false,
                PlayerId = 1
            },
            new()
            {
                Id = 2,
                GameId = 1,
                IsReady = false,
                PlayerId = 2
            }
        };

        A.CallTo(() => _boardRepository.GetAll()).Returns(expectedGetAllBoardsResult.AsQueryable());

        //Act
        var result = _sut.NewBoardAsync(testPlayerId, testGameId);

        //Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.TypeOf<Board>());
        Assert.That(result.Result?.GameId, Is.EqualTo(testGameId));
    }

    [Test]
    public void NewBoardAsync_returns_new_board_if_it_does_NOT_exist()
    {
        //Arrange
        var testPlayerId = 1;
        var testGameId = 1;

        var expectedGetAllBoardsResult = new List<Board>();

        var expectedBoard = new Board
        {
            PlayerId = testPlayerId,
            GameId = testGameId
        };

        A.CallTo(() => _boardRepository.GetAll()).Returns(expectedGetAllBoardsResult.AsQueryable());

        A.CallTo(() => _boardRepository.AddAsync(A<Board>._)).Returns(expectedBoard);

        //Act
        var result = _sut.NewBoardAsync(testPlayerId, testGameId);

        //Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.TypeOf<Board>());
        Assert.That(result.Result?.GameId, Is.EqualTo(testGameId));
    }

    [Test]
    public void AddShipsToBoardAsync_adds_correct_number_of_ships_to_db()
    {
        //Arrange
        const int testPlayerId = 1;
        const string testGameCode = "test1234";

        string[,] testShips =
        {
            { "S", "", "", "", "", "", "" },
            { "", "S", "", "", "", "", "" },
            { "", "", "S", "", "", "", "" },
            { "", "", "", "S", "", "", "" },
            { "", "", "", "", "S", "", "" },
            { "", "", "", "", "", "S", "" },
            { "", "", "", "", "", "", "S" }
        };

        var expectedGetAllGameList = new List<Game>
        {
            new()
            {
                Id = 1,
                GameCode = testGameCode
            }
        };

        var expectedGetAllBoardList = new List<Board>
        {
            new()
            {
                Id = 1,
                PlayerId = testPlayerId,
                GameId = 1
            }
        };

        A.CallTo(() => _gameRepository.GetAll()).Returns(expectedGetAllGameList.AsQueryable());
        A.CallTo(() => _boardRepository.GetAll()).Returns(expectedGetAllBoardList.AsQueryable());

        //Act
        _ = _sut.AddShipsToBoardAsync(testShips, testGameCode, testPlayerId);

        //Assert
        A.CallTo(() => _shipRepository.AddAsync(A<Ship>._))
            .MustHaveHappenedANumberOfTimesMatching(n => n == 7);
    }

    [Test]
    public void GetOpponentAsync_should_return_opponent()
    {
        //Arrange
        const int testPlayerId1 = 1;
        const int testPlayerId2 = 2;
        const int testGameId = 1;

        var expectedGame = new Game
        {
            Id = testGameId,
            Player1Id = testPlayerId1,
            Player2Id = testPlayerId2
        };

        var expectedPlayer1 = new Player
        {
            Id = testPlayerId1
        };

        var expectedPlayer2 = new Player
        {
            Id = testPlayerId2
        };

        A.CallTo(() => _gameRepository
                .GetAsync(An<int>._))
            .Returns(expectedGame);

        A.CallTo(() => _playerRepository
                .GetAsync(An<int>.That.Matches(i => i == testPlayerId1)))
            .Returns(expectedPlayer1);

        A.CallTo(() => _playerRepository
                .GetAsync(An<int>.That.Matches(i => i == testPlayerId2)))
            .Returns(expectedPlayer2);

        //Act
        var result1 = _sut.GetOpponentAsync(testGameId, testPlayerId1);
        var result2 = _sut.GetOpponentAsync(testGameId, testPlayerId2);

        //Assert
        Assert.That(result1.Result?.Id, Is.EqualTo(testPlayerId2));
        Assert.That(result2.Result?.Id, Is.EqualTo(testPlayerId1));
    }

    [Test]
    public void GetBoard_returns_correct_board()
    {
        //Arrange
        var board1 = new Board
        {
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

        //Act
        var result = _sut.GetBoard(1, 1);

        //Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result?.GameId, Is.EqualTo(board1.GameId));
        Assert.That(result?.PlayerId, Is.EqualTo(board1.PlayerId));
    }

    [Test]
    public void GetLastShotAsync()
    {
        //Arrange
        const int testGameId = 1;
        const int lastShotId = 3;

        var expectedGetAllBoardsResult = new List<Board>
        {
            new()
            {
                Id = 1,
                IsReady = true,
                GameId = testGameId,
                PlayerId = 1
            },
            new()
            {
                Id = 2,
                IsReady = true,
                GameId = testGameId,
                PlayerId = 2
            }
        };

        var expectedGetAllShotsResult = new List<Shot>
        {
            new()
            {
                Id = 1,
                BoardId = 1,
                X = 1,
                Y = 1,
                ShotStatus = "hit"
            },
            new()
            {
                Id = lastShotId,
                BoardId = 2,
                X = 3,
                Y = 3,
                ShotStatus = "hit"
            },
            new()
            {
                Id = 2,
                BoardId = 1,
                X = 2,
                Y = 2,
                ShotStatus = "missed"
            }
        };

        A.CallTo(() => _boardRepository.GetAll())
            .Returns(expectedGetAllBoardsResult.AsQueryable());

        A.CallTo(() => _shotRepository.GetAll())
            .Returns(expectedGetAllShotsResult.AsQueryable());

        //Act
        var result = _sut.GetLastShot(testGameId);

        //Assert
        Assert.That(result?.Id, Is.EqualTo(lastShotId));
    }

    [Test]
    public void CheckShot_should_return_hit_if_ship_at_x_y_location()
    {
        //Arrange
        const int testBoardId = 2;
        const int testShipX = 2;
        const int testShipY = 2;

        var expectedBoard = new Board
        {
            Id = 1
        };

        A.CallTo(() => _boardRepository.GetAsync(An<int>._))
            .Returns(expectedBoard);

        var expectedGetAllShipsResult = new List<Ship>
        {
            new()
            {
                Id = 1,
                BoardId = 1,
                PosX = 1,
                PosY = 1
            },
            new()
            {
                Id = 2,
                BoardId = testBoardId,
                PosX = testShipX,
                PosY = testShipY
            },
            new()
            {
                Id = 3,
                BoardId = testBoardId,
                PosX = 3,
                PosY = 3
            }
        };

        var expectedShot = new Shot
        {
            ShotStatus = "hit"
        };

        A.CallTo(() => _shipRepository.GetAll())
            .Returns(expectedGetAllShipsResult.AsQueryable());

        A.CallTo(() => _shotRepository
                .AddAsync(A<Shot>.That.Matches(s => s.ShotStatus == "hit")))
            .Returns(expectedShot);
        //Act
        var result = _sut.CheckShotAsync(testBoardId, testShipX, testShipY);

        //Assert
        Assert.That(result.Result?.ShotStatus, Is.EqualTo("hit"));
    }

    [Test]
    public void CheckShot_should_return_missed_if_ship_NOT_at_x_y_location()
    {
        //Arrange
        const int testBoardId = 2;
        const int testShipX = 2;
        const int testShipY = 2;

        var expectedBoard = new Board
        {
            Id = 1
        };

        A.CallTo(() => _boardRepository.GetAsync(An<int>._))
            .Returns(expectedBoard);

        var expectedGetAllShipsResult = new List<Ship>
        {
            new()
            {
                Id = 1,
                BoardId = 1,
                PosX = 1,
                PosY = 1
            },
            new()
            {
                Id = 3,
                BoardId = testBoardId,
                PosX = 3,
                PosY = 3
            }
        };

        var expectedShot = new Shot
        {
            ShotStatus = "missed"
        };

        A.CallTo(() => _shipRepository.GetAll())
            .Returns(expectedGetAllShipsResult.AsQueryable());

        A.CallTo(() => _shotRepository
                .AddAsync(A<Shot>.That.Matches(s => s.ShotStatus == "missed")))
            .Returns(expectedShot);

        //Act
        var result = _sut.CheckShotAsync(testBoardId, testShipX, testShipY);

        //Assert
        Assert.That(result.Result?.ShotStatus, Is.EqualTo("missed"));
    }

    [Test]
    public void ReadyUpAsync_should_update_board_IsReady_status_to_True()
    {
        //Arrange
        const string testGameCode = "test1234";
        const int testGameId = 1;
        const int testPlayerId = 1;

        var testBoard = new Board
        {
            GameId = testGameId,
            PlayerId = testPlayerId,
            IsReady = false
        };

        var expectedGetAllGameList = new List<Game>
        {
            new()
            {
                Id = testGameId,
                GameCode = testGameCode
            }
        };

        //GetGameByGameCode calls .GetAll()
        A.CallTo(() => _gameRepository.GetAll())
            .Returns(expectedGetAllGameList.AsQueryable());

        var expectedGetAllBoardsList = new List<Board> { testBoard };

        //GetBoard calls .GetAll()
        A.CallTo(() => _boardRepository.GetAll())
            .Returns(expectedGetAllBoardsList.AsQueryable());

        A.CallTo(() => _boardRepository.UpdateAsync(A<Board>._))
            .Returns(testBoard);

        //Act
        _ = _sut.ReadyUpAsync(testGameCode, testPlayerId);

        //Assert
        A.CallTo(() => _boardRepository
                .UpdateAsync(A<Board>.That.Matches(b => b.IsReady == true)))
            .MustHaveHappenedOnceExactly();
    }

    [Test]
    public void GetShipsMatrix()
    {
        //Arrange
        var testBoardId = 1;

        var expectedGetAllShipsResult = new List<Ship>
        {
            new()
            {
                Id = 1,
                BoardId = testBoardId,
                PosX = 1,
                PosY = 1
            },
            new()
            {
                Id = 2,
                BoardId = testBoardId,
                PosX = 2,
                PosY = 2
            },
            new()
            {
                Id = 3,
                BoardId = testBoardId,
                PosX = 3,
                PosY = 3
            },
            new()
            {
                Id = 4,
                BoardId = testBoardId,
                PosX = 4,
                PosY = 4
            }
        };

        A.CallTo(() => _shipRepository.GetAll())
            .Returns(expectedGetAllShipsResult.AsQueryable());

        string[,] expectedShipArray =
        {
            { "", "", "", "", "", "", "" },
            { "", "S", "", "", "", "", "" },
            { "", "", "S", "", "", "", "" },
            { "", "", "", "S", "", "", "" },
            { "", "", "", "", "S", "", "" },
            { "", "", "", "", "", "", "" },
            { "", "", "", "", "", "", "" }
        };

        //Act
        var result = _sut.GetShipsMatrix(testBoardId);

        //Assert
        Assert.That(result, Is.EqualTo(expectedShipArray));
    }

    [Test]
    public void GetLobbyReadyStatusAsync_should_return_true_if_both_players_ready()
    {
        const int testGameId = 5;
        const int testPlayer1Id = 2;
        const int testPlayer2Id = 3;

        //Arrange
        var expectedGame = new Game
        {
            Id = testGameId,
            Player1Id = testPlayer1Id,
            Player2Id = testPlayer2Id
        };

        A.CallTo(() => _gameRepository.GetAsync(An<int>._))
            .Returns(expectedGame);

        var expectedGetAllBoardsResult = new List<Board>
        {
            new()
            {
                Id = 1,
                GameId = testGameId,
                PlayerId = testPlayer1Id,
                IsReady = true
            },
            new()
            {
                Id = 2,
                GameId = testGameId,
                PlayerId = testPlayer2Id,
                IsReady = true
            }
        };

        A.CallTo(() => _boardRepository.GetAll())
            .Returns(expectedGetAllBoardsResult.AsQueryable());

        //Act
        var result = _sut.GetLobbyReadyStatusAsync(testGameId);

        //Assert
        Assert.That(result.Result, Is.True);
    }

    [Test]
    public void GetLobbyReadyStatusAsync_should_return_false_if_one_player_is_NOT_ready()
    {
        const int testGameId = 5;
        const int testPlayer1Id = 2;
        const int testPlayer2Id = 3;

        //Arrange
        var expectedGame = new Game
        {
            Id = testGameId,
            Player1Id = testPlayer1Id,
            Player2Id = testPlayer2Id
        };

        A.CallTo(() => _gameRepository.GetAsync(An<int>._))
            .Returns(expectedGame);

        var expectedGetAllBoardsResult = new List<Board>
        {
            new()
            {
                Id = 1,
                GameId = testGameId,
                PlayerId = testPlayer1Id,
                IsReady = true
            },
            new()
            {
                Id = 2,
                GameId = testGameId,
                PlayerId = testPlayer2Id,
                IsReady = false
            }
        };

        A.CallTo(() => _boardRepository.GetAll())
            .Returns(expectedGetAllBoardsResult.AsQueryable());

        //Act
        var result = _sut.GetLobbyReadyStatusAsync(testGameId);

        //Assert
        Assert.That(result.Result, Is.False);
    }

    [Test]
    public void GetLobbyReadyStatusAsync_should_return_false_if_player2_has_not_joined_game_yet()
    {
        const int testGameId = 5;
        const int testPlayer1Id = 2;

        //Arrange
        var expectedGame = new Game
        {
            Id = testGameId,
            Player1Id = testPlayer1Id
        };

        A.CallTo(() => _gameRepository.GetAsync(An<int>._))
            .Returns(expectedGame);

        var expectedGetAllBoardsResult = new List<Board>
        {
            new()
            {
                Id = 1,
                GameId = testGameId,
                PlayerId = testPlayer1Id,
                IsReady = true
            }
        };

        A.CallTo(() => _boardRepository.GetAll())
            .Returns(expectedGetAllBoardsResult.AsQueryable());

        //Act
        var result = _sut.GetLobbyReadyStatusAsync(testGameId);

        //Assert
        Assert.That(result.Result, Is.False);
    }

    [Test]
    public void SetGameStartedDateTime()
    {
        //Arrange

        //Act

        //Assert
    }
}