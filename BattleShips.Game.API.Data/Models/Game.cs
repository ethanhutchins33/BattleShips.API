namespace BattleShips.Game.API.Data.Models
{
    public class Game
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }
        public Player Player1Id { get; set; }
        public Player Player2Id { get; set; }
        public int PlayerWinnerId { get; set; }

        public Game()
        {
            DateCreated = DateTime.Now;
            Player1Id = new Player();
            Player2Id = new Player();
        }
    }
}
