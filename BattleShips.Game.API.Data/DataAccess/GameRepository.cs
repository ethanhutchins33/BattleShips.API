using Microsoft.EntityFrameworkCore;

namespace BattleShips.Game.API.Data.DataAccess;
public class GameRepository : IGameRepository
{
    private readonly GameContext _db;

    public GameRepository(GameContext db)
    {
        _db = db;
    }

    #region Models.Game

    public async Task<Models.Game> GetGameAsync(int id)
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

    public async Task<Models.Game> CreateGameAsync(Models.Game game)
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

    public async Task<Models.Game> UpdateGameAsync(Models.Game game)
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

    public async Task<(bool, string)> DeleteGameAsync(Models.Game game)
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

    #endregion Models.Game

    #region Player

    

    #endregion Player


    #region Board


    #endregion Board


    #region Ship


    #endregion Ship
}
