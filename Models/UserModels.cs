namespace Game2048.Models
{
    public class User
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public Dictionary<int, int> HighScores { get; set; } = new Dictionary<int, int>
        {
            { 3, 0 },
            { 4, 0 },
            { 5, 0 }
        };
        // SavedGames removed - causes JSON serialization issues with int[,] arrays
        // public Dictionary<int, GameState> SavedGames { get; set; } = new Dictionary<int, GameState>();
    }

    public class GameState
    {
        public int[,] Board { get; set; } = new int[4, 4];
        public int Score { get; set; }
        public int GridSize { get; set; }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public class MoveRequest
    {
        public string Direction { get; set; } = "";
    }

    public class GameResponse
    {
        public int[][] Board { get; set; } = new int[4][];
        public int Score { get; set; }
        public int BestScore { get; set; }
        public bool GameOver { get; set; }
        public bool Won { get; set; }
    }
}
