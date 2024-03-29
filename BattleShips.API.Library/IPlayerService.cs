﻿using BattleShips.API.Data.Models;

namespace BattleShips.API.Library;

public interface IPlayerService
{
    Player? Get(Guid azureId);
    Task<Player?> GetAsync(int id);
    Task<Player?> AddAsync(Guid azureId, string userName);
    Task<Player?> RemoveAsync(int id);
}