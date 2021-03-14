namespace Course_project.Models
{
    public class TicTacPlayer
    {
        public TicTacPlayer Opponent { get; set; }

        public bool WaitingForMove { get; set; }
        public string ConnectionId { get; set; }
        public string game { get; set; }
        public string Email { get; set; }
    }
}
