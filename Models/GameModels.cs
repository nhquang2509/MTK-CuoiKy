namespace Game2048.Models
{
    // ============================================
    // DESIGN PATTERN 1: SINGLETON PATTERN
    // Quản lý trạng thái game (board, score, moves)
    // Đảm bảo chỉ có 1 instance duy nhất trong suốt game
    // ============================================
    public sealed class GameManagerSingleton
    {
        private static GameManagerSingleton? _instance;
        private static readonly object _lock = new object();

        public int GridSize { get; private set; }
        public int[,] Board { get; private set; }
        public int Score { get; private set; }
        public int BestScore { get; private set; }
        public string Username { get; private set; }
        public IMoveStrategy MoveStrategy { get; set; }

        // Private constructor để ngăn tạo instance từ bên ngoài
        private GameManagerSingleton()
        {
            GridSize = 4;
            Board = new int[4, 4];
            Score = 0;
            BestScore = 0;
            Username = "";
            MoveStrategy = new StandardMoveStrategy(); // Default strategy
        }

        // Singleton Instance - Thread-safe
        public static GameManagerSingleton Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new GameManagerSingleton();
                        }
                    }
                }
                return _instance;
            }
        }

        // Khởi tạo game mới với grid size
        public void InitializeGame(int gridSize, string username)
        {
            GridSize = gridSize;
            Board = new int[gridSize, gridSize];
            Score = 0;
            Username = username;
            AddRandomTile();
            AddRandomTile();
        }

        // Thêm ô ngẫu nhiên (2 hoặc 4)
        public void AddRandomTile()
        {
            var emptyCells = new List<(int, int)>();
            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    if (Board[i, j] == 0)
                        emptyCells.Add((i, j));
                }
            }

            if (emptyCells.Count > 0)
            {
                var random = new Random();
                var (row, col) = emptyCells[random.Next(emptyCells.Count)];
                Board[row, col] = random.Next(10) < 9 ? 2 : 4; // 90% là 2, 10% là 4
            }
        }

        // Di chuyển sử dụng Strategy Pattern
        public bool Move(Direction direction)
        {
            var (newBoard, scoreDelta) = MoveStrategy.ExecuteMove(Board, GridSize, direction);
            
            bool moved = !BoardsEqual(Board, newBoard);
            if (moved)
            {
                Board = newBoard;
                Score += scoreDelta;
                AddRandomTile();
                
                if (Score > BestScore)
                    BestScore = Score;
            }

            return moved;
        }

        private bool BoardsEqual(int[,] board1, int[,] board2)
        {
            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    if (board1[i, j] != board2[i, j])
                        return false;
                }
            }
            return true;
        }

        public bool IsGameOver()
        {
            // Kiểm tra có ô trống
            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    if (Board[i, j] == 0)
                        return false;
                }
            }

            // Kiểm tra có thể merge
            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    if (j < GridSize - 1 && Board[i, j] == Board[i, j + 1])
                        return false;
                    if (i < GridSize - 1 && Board[i, j] == Board[i + 1, j])
                        return false;
                }
            }

            return true;
        }

        public bool HasWon()
        {
            int target = GridSize == 3 ? 128 : GridSize == 4 ? 2048 : 4096;
            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    if (Board[i, j] >= target)
                        return true;
                }
            }
            return false;
        }

        public void UpdateBestScore(int bestScore)
        {
            BestScore = Math.Max(BestScore, bestScore);
        }
    }

    // ============================================
    // DESIGN PATTERN 2: DECORATOR PATTERN
    // Trang trí Cell với các tính năng bổ sung
    // (hiệu ứng, màu sắc, thông báo) mà không thay đổi Cell cơ bản
    // ============================================

    // Component cơ bản
    public interface ICell
    {
        int Value { get; set; }
        string GetDisplay();
        string GetCssClass();
    }

    // Concrete Component
    public class Cell : ICell
    {
        public int Value { get; set; }

        public Cell(int value)
        {
            Value = value;
        }

        public virtual string GetDisplay()
        {
            return Value == 0 ? "" : Value.ToString();
        }

        public virtual string GetCssClass()
        {
            return $"cell tile-{Value}";
        }
    }

    // Base Decorator
    public abstract class CellDecorator : ICell
    {
        protected ICell _cell;

        public CellDecorator(ICell cell)
        {
            _cell = cell;
        }

        public int Value
        {
            get => _cell.Value;
            set => _cell.Value = value;
        }

        public virtual string GetDisplay()
        {
            return _cell.GetDisplay();
        }

        public virtual string GetCssClass()
        {
            return _cell.GetCssClass();
        }
    }

    // Concrete Decorator 1: Thêm hiệu ứng special cho ô đạt mốc cao
    public class SpecialEffectDecorator : CellDecorator
    {
        public SpecialEffectDecorator(ICell cell) : base(cell) { }

        public override string GetCssClass()
        {
            string baseClass = base.GetCssClass();
            if (Value >= 512)
            {
                return baseClass + " special-glow"; // Thêm hiệu ứng glow
            }
            return baseClass;
        }

        public override string GetDisplay()
        {
            string baseDisplay = base.GetDisplay();
            if (Value >= 1024)
            {
                return "🏆 " + baseDisplay; // Thêm icon trophy
            }
            return baseDisplay;
        }
    }

    // Concrete Decorator 2: Thêm animation cho ô mới
    public class NewTileDecorator : CellDecorator
    {
        public NewTileDecorator(ICell cell) : base(cell) { }

        public override string GetCssClass()
        {
            return base.GetCssClass() + " new-tile"; // Thêm animation class
        }
    }

    // Concrete Decorator 3: Thêm notification khi merge
    public class MergedTileDecorator : CellDecorator
    {
        private string _notification;

        public MergedTileDecorator(ICell cell, string notification = "Merged!") : base(cell)
        {
            _notification = notification;
        }

        public override string GetCssClass()
        {
            return base.GetCssClass() + " merged-tile"; // Thêm animation merge
        }

        public override string GetDisplay()
        {
            return base.GetDisplay(); // Có thể thêm badge "+{value}"
        }
    }

    // ============================================
    // DESIGN PATTERN 3: STRATEGY PATTERN
    // Các thuật toán khác nhau cho di chuyển và merge ô
    // ============================================

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    // Strategy Interface
    public interface IMoveStrategy
    {
        (int[,] newBoard, int scoreDelta) ExecuteMove(int[,] board, int gridSize, Direction direction);
    }

    // Concrete Strategy 1: Standard Move (thuật toán chuẩn)
    public class StandardMoveStrategy : IMoveStrategy
    {
        public (int[,] newBoard, int scoreDelta) ExecuteMove(int[,] board, int gridSize, Direction direction)
        {
            int[,] newBoard = (int[,])board.Clone();
            int scoreDelta = 0;

            switch (direction)
            {
                case Direction.Left:
                    scoreDelta = MoveLeft(newBoard, gridSize);
                    break;
                case Direction.Right:
                    scoreDelta = MoveRight(newBoard, gridSize);
                    break;
                case Direction.Up:
                    scoreDelta = MoveUp(newBoard, gridSize);
                    break;
                case Direction.Down:
                    scoreDelta = MoveDown(newBoard, gridSize);
                    break;
            }

            return (newBoard, scoreDelta);
        }

        private int MoveLeft(int[,] board, int gridSize)
        {
            int score = 0;
            for (int i = 0; i < gridSize; i++)
            {
                score += ProcessRow(board, i, gridSize, true);
            }
            return score;
        }

        private int MoveRight(int[,] board, int gridSize)
        {
            int score = 0;
            for (int i = 0; i < gridSize; i++)
            {
                score += ProcessRow(board, i, gridSize, false);
            }
            return score;
        }

        private int MoveUp(int[,] board, int gridSize)
        {
            int score = 0;
            for (int j = 0; j < gridSize; j++)
            {
                score += ProcessColumn(board, j, gridSize, true);
            }
            return score;
        }

        private int MoveDown(int[,] board, int gridSize)
        {
            int score = 0;
            for (int j = 0; j < gridSize; j++)
            {
                score += ProcessColumn(board, j, gridSize, false);
            }
            return score;
        }

        private int ProcessRow(int[,] board, int row, int gridSize, bool leftToRight)
        {
            int[] line = new int[gridSize];
            for (int i = 0; i < gridSize; i++)
            {
                line[i] = board[row, leftToRight ? i : gridSize - 1 - i];
            }

            int score = MergeLine(line);

            for (int i = 0; i < gridSize; i++)
            {
                board[row, leftToRight ? i : gridSize - 1 - i] = line[i];
            }

            return score;
        }

        private int ProcessColumn(int[,] board, int col, int gridSize, bool topToBottom)
        {
            int[] line = new int[gridSize];
            for (int i = 0; i < gridSize; i++)
            {
                line[i] = board[topToBottom ? i : gridSize - 1 - i, col];
            }

            int score = MergeLine(line);

            for (int i = 0; i < gridSize; i++)
            {
                board[topToBottom ? i : gridSize - 1 - i, col] = line[i];
            }

            return score;
        }

        private int MergeLine(int[] line)
        {
            int score = 0;
            
            // Slide non-zero values to the left
            int[] temp = line.Where(x => x != 0).ToArray();
            
            // Merge adjacent equal values
            for (int i = 0; i < temp.Length - 1; i++)
            {
                if (temp[i] == temp[i + 1] && temp[i] != 0)
                {
                    temp[i] *= 2;
                    score += temp[i];
                    temp[i + 1] = 0;
                }
            }

            // Slide again after merging
            int[] result = temp.Where(x => x != 0).ToArray();
            
            // Fill the line
            for (int i = 0; i < line.Length; i++)
            {
                line[i] = i < result.Length ? result[i] : 0;
            }

            return score;
        }
    }

    // Concrete Strategy 2: Fast Mode (merge nhiều lần trong 1 lượt)
    public class FastMoveStrategy : IMoveStrategy
    {
        public (int[,] newBoard, int scoreDelta) ExecuteMove(int[,] board, int gridSize, Direction direction)
        {
            // Thực hiện merge mạnh hơn - cho phép merge nhiều lần
            var standardStrategy = new StandardMoveStrategy();
            var (newBoard, score1) = standardStrategy.ExecuteMove(board, gridSize, direction);
            var (finalBoard, score2) = standardStrategy.ExecuteMove(newBoard, gridSize, direction);
            
            return (finalBoard, score1 + score2);
        }
    }

    // Concrete Strategy 3: Test Mode (không merge, chỉ di chuyển)
    public class TestMoveStrategy : IMoveStrategy
    {
        public (int[,] newBoard, int scoreDelta) ExecuteMove(int[,] board, int gridSize, Direction direction)
        {
            int[,] newBoard = (int[,])board.Clone();
            
            // Chỉ di chuyển, không merge
            switch (direction)
            {
                case Direction.Left:
                    MoveOnlyLeft(newBoard, gridSize);
                    break;
                case Direction.Right:
                    MoveOnlyRight(newBoard, gridSize);
                    break;
                case Direction.Up:
                    MoveOnlyUp(newBoard, gridSize);
                    break;
                case Direction.Down:
                    MoveOnlyDown(newBoard, gridSize);
                    break;
            }

            return (newBoard, 0); // Không tính điểm
        }

        private void MoveOnlyLeft(int[,] board, int gridSize)
        {
            for (int i = 0; i < gridSize; i++)
            {
                var values = new List<int>();
                for (int j = 0; j < gridSize; j++)
                {
                    if (board[i, j] != 0)
                        values.Add(board[i, j]);
                }
                for (int j = 0; j < gridSize; j++)
                {
                    board[i, j] = j < values.Count ? values[j] : 0;
                }
            }
        }

        private void MoveOnlyRight(int[,] board, int gridSize)
        {
            for (int i = 0; i < gridSize; i++)
            {
                var values = new List<int>();
                for (int j = gridSize - 1; j >= 0; j--)
                {
                    if (board[i, j] != 0)
                        values.Add(board[i, j]);
                }
                for (int j = gridSize - 1; j >= 0; j--)
                {
                    board[i, j] = (gridSize - 1 - j) < values.Count ? values[gridSize - 1 - j] : 0;
                }
            }
        }

        private void MoveOnlyUp(int[,] board, int gridSize)
        {
            for (int j = 0; j < gridSize; j++)
            {
                var values = new List<int>();
                for (int i = 0; i < gridSize; i++)
                {
                    if (board[i, j] != 0)
                        values.Add(board[i, j]);
                }
                for (int i = 0; i < gridSize; i++)
                {
                    board[i, j] = i < values.Count ? values[i] : 0;
                }
            }
        }

        private void MoveOnlyDown(int[,] board, int gridSize)
        {
            for (int j = 0; j < gridSize; j++)
            {
                var values = new List<int>();
                for (int i = gridSize - 1; i >= 0; i--)
                {
                    if (board[i, j] != 0)
                        values.Add(board[i, j]);
                }
                for (int i = gridSize - 1; i >= 0; i--)
                {
                    board[i, j] = (gridSize - 1 - i) < values.Count ? values[gridSize - 1 - i] : 0;
                }
            }
        }
    }
}
