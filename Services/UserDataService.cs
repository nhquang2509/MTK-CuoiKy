using System.Text.Json;
using Game2048.Models;

namespace Game2048.Services
{
    /// <summary>
    /// Service để quản lý lưu trữ và đọc dữ liệu users từ JSON file
    /// </summary>
    public class UserDataService
    {
        private readonly string _dataFilePath;
        private readonly object _lock = new object();

        public UserDataService()
        {
            // Đường dẫn đến file JSON trong thư mục Data
            _dataFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "users.json");
            
            // Tạo thư mục Data nếu chưa có
            var directory = Path.GetDirectoryName(_dataFilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Tạo file rỗng nếu chưa tồn tại
            if (!File.Exists(_dataFilePath))
            {
                Console.WriteLine($"[DATA] Creating new users.json file at: {_dataFilePath}");
                SaveUsers(new List<User>());
            }
            else
            {
                // Load và hiển thị số lượng users
                var users = LoadUsers();
                Console.WriteLine($"[DATA] Loaded {users.Count} users from {_dataFilePath}");
                foreach (var user in users)
                {
                    Console.WriteLine($"       - {user.Username}");
                }
            }
        }

        /// <summary>
        /// Đọc tất cả users từ file JSON
        /// </summary>
        public List<User> LoadUsers()
        {
            lock (_lock)
            {
                try
                {
                    if (!File.Exists(_dataFilePath))
                    {
                        return new List<User>();
                    }

                    var json = File.ReadAllText(_dataFilePath);
                    var users = JsonSerializer.Deserialize<List<User>>(json);
                    return users ?? new List<User>();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Failed to load users: {ex.Message}");
                    return new List<User>();
                }
            }
        }

        /// <summary>
        /// Lưu tất cả users vào file JSON
        /// </summary>
        public void SaveUsers(List<User> users)
        {
            lock (_lock)
            {
                try
                {
                    var options = new JsonSerializerOptions
                    {
                        WriteIndented = true // Format đẹp, dễ đọc
                    };

                    var json = JsonSerializer.Serialize(users, options);
                    File.WriteAllText(_dataFilePath, json);
                    
                    Console.WriteLine($"[DATA] Saved {users.Count} users to {_dataFilePath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Failed to save users: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Lấy user theo username
        /// </summary>
        public User? GetUser(string username)
        {
            var users = LoadUsers();
            var user = users.FirstOrDefault(u => u.Username == username);
            
            if (user == null)
            {
                Console.WriteLine($"[DATA] User '{username}' NOT FOUND in database");
            }
            else
            {
                Console.WriteLine($"[DATA] User '{username}' found in database");
            }
            
            return user;
        }

        /// <summary>
        /// Cập nhật hoặc thêm user mới
        /// </summary>
        public void SaveUser(User user)
        {
            lock (_lock)
            {
                var users = LoadUsers();
                var existingUser = users.FirstOrDefault(u => u.Username == user.Username);

                if (existingUser != null)
                {
                    // Cập nhật user hiện có
                    existingUser.Password = user.Password;
                    existingUser.HighScores = user.HighScores;
                }
                else
                {
                    // Thêm user mới
                    users.Add(user);
                }

                SaveUsers(users);
            }
        }

        /// <summary>
        /// Cập nhật high score cho user
        /// </summary>
        public void UpdateHighScore(string username, int gridSize, int score)
        {
            lock (_lock)
            {
                var users = LoadUsers();
                var user = users.FirstOrDefault(u => u.Username == username);

                if (user != null)
                {
                    if (!user.HighScores.ContainsKey(gridSize))
                    {
                        user.HighScores[gridSize] = 0;
                    }

                    if (score > user.HighScores[gridSize])
                    {
                        user.HighScores[gridSize] = score;
                        SaveUsers(users);
                        
                        Console.WriteLine($"[DATA] Updated high score for {username}: {gridSize}x{gridSize} = {score}");
                    }
                }
            }
        }

        /// <summary>
        /// Lấy top players theo grid size
        /// </summary>
        public List<LeaderboardEntry> GetLeaderboard(int gridSize, int topCount = 10)
        {
            var users = LoadUsers();

            var leaderboard = users
                .Where(u => u.HighScores.ContainsKey(gridSize))
                .Select(u => new LeaderboardEntry
                {
                    Username = u.Username,
                    Score = u.HighScores[gridSize]
                })
                .OrderByDescending(e => e.Score)
                .Take(topCount)
                .ToList();

            return leaderboard;
        }
    }

    public class LeaderboardEntry
    {
        public string Username { get; set; } = string.Empty;
        public int Score { get; set; }
    }
}
