using Game2048.Models;
using Microsoft.AspNetCore.Mvc;

namespace Game2048.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        // Simulated user database (in production, use real database)
        private static Dictionary<string, User> _users = new Dictionary<string, User>
        {
            { "player1", new User { Username = "player1", Password = "pass123" } },
            { "player2", new User { Username = "player2", Password = "pass456" } }
        };

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            Console.WriteLine($"[LOGIN] Attempting login for user: {request.Username}");
            
            if (_users.TryGetValue(request.Username, out var user))
            {
                if (user.Password == request.Password)
                {
                    HttpContext.Session.SetString("Username", user.Username);
                    
                    // Force session to be saved
                    HttpContext.Session.CommitAsync().Wait();
                    
                    Console.WriteLine($"[LOGIN] Success! Session ID: {HttpContext.Session.Id}");
                    Console.WriteLine($"[LOGIN] Username stored in session: {HttpContext.Session.GetString("Username")}");
                    
                    return Ok(new
                    {
                        success = true,
                        username = user.Username,
                        highScores = user.HighScores
                    });
                }
            }

            Console.WriteLine($"[LOGIN] Failed for user: {request.Username}");
            return Unauthorized(new { success = false, message = "Invalid credentials" });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Ok(new { success = true });
        }

        [HttpGet("current")]
        public IActionResult GetCurrentUser()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized(new { success = false });
            }

            if (_users.TryGetValue(username, out var user))
            {
                return Ok(new
                {
                    success = true,
                    username = user.Username,
                    highScores = user.HighScores
                });
            }

            return NotFound();
        }

        [HttpPost("save-score")]
        public IActionResult SaveScore([FromBody] SaveScoreRequest request)
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized();
            }

            if (_users.TryGetValue(username, out var user))
            {
                if (!user.HighScores.ContainsKey(request.GridSize))
                {
                    user.HighScores[request.GridSize] = 0;
                }

                if (request.Score > user.HighScores[request.GridSize])
                {
                    user.HighScores[request.GridSize] = request.Score;
                }

                return Ok(new { success = true, highScores = user.HighScores });
            }

            return NotFound();
        }
    }

    public class SaveScoreRequest
    {
        public int GridSize { get; set; }
        public int Score { get; set; }
    }
}
