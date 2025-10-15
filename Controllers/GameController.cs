using Game2048.Models;
using Microsoft.AspNetCore.Mvc;

namespace Game2048.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        // Sử dụng Singleton Pattern để quản lý game state
        private GameManagerSingleton _gameManager = GameManagerSingleton.Instance;

        // Helper method để convert 2D array sang jagged array
        private int[][] ConvertToJaggedArray(int[,] board, int gridSize)
        {
            var result = new int[gridSize][];
            for (int i = 0; i < gridSize; i++)
            {
                result[i] = new int[gridSize];
                for (int j = 0; j < gridSize; j++)
                {
                    result[i][j] = board[i, j];
                }
            }
            return result;
        }

        [HttpPost("new-game")]
        public IActionResult NewGame([FromBody] NewGameRequest request)
        {
            Console.WriteLine($"[NEW-GAME] Session ID: {HttpContext.Session.Id}");
            
            var username = HttpContext.Session.GetString("Username");
            Console.WriteLine($"[NEW-GAME] Username from session: {username ?? "NULL"}");
            
            if (string.IsNullOrEmpty(username))
            {
                Console.WriteLine("[NEW-GAME] Unauthorized - No username in session");
                return Unauthorized(new { success = false, message = "Not logged in" });
            }

            // Khởi tạo game mới với grid size
            _gameManager.InitializeGame(request.GridSize, username);
            
            Console.WriteLine($"[NEW-GAME] Game initialized for {username}, GridSize: {request.GridSize}");

            return Ok(new GameResponse
            {
                Board = ConvertToJaggedArray(_gameManager.Board, _gameManager.GridSize),
                Score = _gameManager.Score,
                BestScore = _gameManager.BestScore,
                GameOver = false,
                Won = false
            });
        }

        [HttpPost("move")]
        public IActionResult Move([FromBody] MoveRequest request)
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized();
            }

            Direction direction;
            switch (request.Direction.ToLower())
            {
                case "up":
                    direction = Direction.Up;
                    break;
                case "down":
                    direction = Direction.Down;
                    break;
                case "left":
                    direction = Direction.Left;
                    break;
                case "right":
                    direction = Direction.Right;
                    break;
                default:
                    return BadRequest(new { message = "Invalid direction" });
            }

            bool moved = _gameManager.Move(direction);

            return Ok(new GameResponse
            {
                Board = ConvertToJaggedArray(_gameManager.Board, _gameManager.GridSize),
                Score = _gameManager.Score,
                BestScore = _gameManager.BestScore,
                GameOver = _gameManager.IsGameOver(),
                Won = _gameManager.HasWon()
            });
        }

        [HttpGet("state")]
        public IActionResult GetState()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized();
            }

            return Ok(new GameResponse
            {
                Board = ConvertToJaggedArray(_gameManager.Board, _gameManager.GridSize),
                Score = _gameManager.Score,
                BestScore = _gameManager.BestScore,
                GameOver = _gameManager.IsGameOver(),
                Won = _gameManager.HasWon()
            });
        }

        [HttpPost("set-strategy")]
        public IActionResult SetStrategy([FromBody] StrategyRequest request)
        {
            IMoveStrategy strategy;
            switch (request.StrategyType.ToLower())
            {
                case "standard":
                    strategy = new StandardMoveStrategy();
                    break;
                case "fast":
                    strategy = new FastMoveStrategy();
                    break;
                case "test":
                    strategy = new TestMoveStrategy();
                    break;
                default:
                    return BadRequest(new { message = "Invalid strategy type" });
            }

            _gameManager.MoveStrategy = strategy;
            return Ok(new { success = true, strategy = request.StrategyType });
        }
    }

    public class NewGameRequest
    {
        public int GridSize { get; set; }
    }

    public class StrategyRequest
    {
        public string StrategyType { get; set; } = "";
    }
}
