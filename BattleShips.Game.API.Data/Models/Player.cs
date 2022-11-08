using System.ComponentModel.DataAnnotations;

namespace BattleShips.Game.API.Data.Models;
public class Player
{
    [Key]
    public int Id { get; set; }
    public string UserName { get; set; }
}
