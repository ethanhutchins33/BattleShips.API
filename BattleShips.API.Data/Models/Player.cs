using System.ComponentModel.DataAnnotations;
using BattleShips.API.Data.Access;

namespace BattleShips.API.Data.Models;
public class Player : IEntity
{
    [Key]
    public int Id { get; set; }
    public string UserName { get; set; }
}
