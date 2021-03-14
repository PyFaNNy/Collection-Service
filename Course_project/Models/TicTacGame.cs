namespace Course_project.Models
{
    public class TicTacGame
    {
        public string Name { get; set; }
        public TicTacPlayer Player1 { get; set; }
        public TicTacPlayer Player2 { get; set; }
        public bool IsOver { get; private set; }
        public int[] field = new int[9];
        private int movesLeft = 9;

        public TicTacGame()
        {
            for (var i = 0; i < field.Length; i++)
            {
                field[i] = -1;
            }
        }
        public bool Play(int player, int position)
        {
            if (this.IsOver)
            {
                return false;
            }
            this.PlacePlayerNumber(player, position);

            return this.CheckWinner();
        }
        private bool CheckWinner()
        {
            for (int i = 0; i < 3; i++)
            {
                if (((field[i * 3] != -1 && field[(i * 3)] == field[(i * 3) + 1] && field[(i * 3)] == field[(i * 3) + 2]) ||
                     (field[i] != -1 && field[i] == field[i + 3] && field[i] == field[i + 6])))
                {
                    this.IsOver = true;
                    return true;
                }
            }

            if ((field[0] != -1 && field[0] == field[4] && field[0] == field[8]) || (field[2] != -1 && field[2] == field[4] && field[2] == field[6]))
            {
                this.IsOver = true;
                return true;
            }

            return false;
        }
        private void PlacePlayerNumber(int player, int position)
        {
            this.movesLeft -= 1;

            if (this.movesLeft <= 0)
            {
                this.IsOver = true;
            }

            if (position < field.Length && field[position] == -1)
            {
                field[position] = player;
            }
        }
    }
}
