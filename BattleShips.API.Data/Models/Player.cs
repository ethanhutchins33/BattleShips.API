using System.ComponentModel.DataAnnotations;
using BattleShips.API.Data.Access;

namespace BattleShips.API.Data.Models;

public class Player : IEntity
{
    public string UserName { get; set; } = string.Empty;
    public Guid AzureId { get; set; }

    [Key] public int Id { get; set; }
}