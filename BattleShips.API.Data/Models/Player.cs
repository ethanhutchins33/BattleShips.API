using System.ComponentModel.DataAnnotations;
using BattleShips.API.Data.Access;

namespace BattleShips.API.Data.Models;

public class Player : IEntity
{
    public string UserName { get; init; } = string.Empty;
    public Guid AzureId { get; init; }

    [Key] public int Id { get; set; }
}