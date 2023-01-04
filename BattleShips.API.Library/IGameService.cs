﻿using BattleShips.API.Data.Models;

namespace BattleShips.API.Library;

public interface IGameService
{
    Task<Game?> SetupNewGame(int playerId); //TODO rename to async
    Task<Game?> AddPlayerToGame(int joiningPlayerId, int gameId);
    Task<Board?> AddBoard(int playerId, int gameId);
    Task<Ship?> AddShipToBoard(Ship ship, int gameId, int playerId);
    Game? GetGameByGameCode(string gameCode);
    //Task<String> CheckShot(int boardId, int rowNumber, char cellValue);
}