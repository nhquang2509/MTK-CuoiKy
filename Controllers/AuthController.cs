using Game2048.Models;
using Game2048.Services;
using Microsoft.AspNetCore.Mvc;

namespace Game2048.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserDataService _userDataService;

        public AuthController(UserDataService userDataService)
        {
            _userDataService = userDataService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            Console.WriteLine($"[LOGIN] Attempting login for user: {request.Username}");
            
            var user = _userDataService.GetUser(request.Username);
            if (user != null)
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

            var user = _userDataService.GetUser(username);
            if (user != null)
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

            var user = _userDataService.GetUser(username);
            if (user != null)
            {
                // Cập nhật high score thông qua service
                _userDataService.UpdateHighScore(username, request.GridSize, request.Score);

                // Lấy lại user data đã cập nhật
                user = _userDataService.GetUser(username);
                
                return Ok(new { success = true, highScores = user?.HighScores });
            }

            return NotFound();
        }

        [HttpGet("leaderboard")]
        public IActionResult GetLeaderboard([FromQuery] int gridSize = 4, [FromQuery] int top = 10)
        {
            var leaderboard = _userDataService.GetLeaderboard(gridSize, top);
            return Ok(new { success = true, leaderboard });
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            Console.WriteLine($"[REGISTER] Attempting to register user: {request.Username}");
            
            // Check if user already exists
            var existingUser = _userDataService.GetUser(request.Username);
            if (existingUser != null)
            {
                return BadRequest(new { success = false, message = "Username already exists" });
            }

            // Create new user
            var newUser = new User
            {
                Username = request.Username,
                Password = request.Password,
                HighScores = new Dictionary<int, int>
                {
                    { 3, 0 },
                    { 4, 0 },
                    { 5, 0 }
                }
            };

            _userDataService.SaveUser(newUser);
            
            Console.WriteLine($"[REGISTER] Successfully registered user: {request.Username}");
            
            return Ok(new { success = true, message = "Registration successful" });
        }
    }

    public class RegisterRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class SaveScoreRequest
    {
        public int GridSize { get; set; }
        public int Score { get; set; }
    }
}
