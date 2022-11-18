using BattleShips.API.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BattleShips.API.Data.DataAccess;
public class Repository : IRepository
{
    private readonly GameContext _db;

    public Repository(GameContext db)
    {
        _db = db;
    }

    public async Task<Game> GetGameAsync(int id)
    {
        try
        {
            return await _db.Games.FirstOrDefaultAsync(game => game.Id == id);
        }
        catch (Exception ex)
        {
            return null;
        }

    }

    public async Task<Game> AddGameAsync(Game game)
    {
        try
        {
            await _db.Games.AddAsync(game);
            await _db.SaveChangesAsync();
            return await _db.Games.FindAsync(game.Id);
        }
        catch (Exception ex)
        {
            return null;
        }

    }

    public async Task<Game> UpdateGameAsync(Game game)
    {
        try
        {
            _db.Entry(game).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            return game;
        }
        catch (Exception ex)
        {
            return null;
        }

    }

    public async Task<(bool, string)> DeleteGameAsync(Game game)
    {
        try
        {
            var dbGame = await _db.Games.FindAsync(game.Id);

            if (dbGame == null)
            {
                return (false, "Game could not be found");
            }

            _db.Games.Remove(game);
            await _db.SaveChangesAsync();

            return (true, "Game was deleted.");
        }
        catch (Exception ex)
        {
            return (false, $"An error occurred. Error Message: {ex.Message}");
        }

    }

  
}
