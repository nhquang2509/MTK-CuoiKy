# 📁 Data Storage - Game 2048

## Mô tả

Thư mục này chứa file `users.json` để lưu trữ **persistent data** cho game, bao gồm:
- Thông tin người dùng (username, password)
- Điểm cao nhất (high scores) cho mỗi kích thước bàn chơi (3x3, 4x4, 5x5)
- Các game đã lưu

## File users.json

### Cấu trúc

```json
[
  {
    "Username": "player1",
    "Password": "pass123",
    "HighScores": {
      "3": 128,
      "4": 2048,
      "5": 512
    },
    "SavedGames": {}
  }
]
```

### Giải thích các trường:

| Trường | Kiểu | Mô tả |
|--------|------|-------|
| `Username` | string | Tên đăng nhập (unique) |
| `Password` | string | Mật khẩu (trong production nên hash) |
| `HighScores` | object | Dictionary lưu điểm cao theo grid size |
| `SavedGames` | object | Dictionary lưu trạng thái game |

### HighScores Structure

```json
"HighScores": {
  "3": 128,   // Điểm cao nhất cho bàn 3x3
  "4": 2048,  // Điểm cao nhất cho bàn 4x4
  "5": 512    // Điểm cao nhất cho bàn 5x5
}
```

## Cách hoạt động

### 1. Load Data (khi khởi động)

```csharp
var userDataService = new UserDataService();
var users = userDataService.LoadUsers();
```

### 2. Save Score (khi game over)

```csharp
userDataService.UpdateHighScore("player1", 4, 2048);
// Tự động lưu vào users.json nếu 2048 > điểm hiện tại
```

### 3. Get Leaderboard

```csharp
var top10 = userDataService.GetLeaderboard(gridSize: 4, topCount: 10);
```

## API Endpoints

### Save Score
```http
POST /api/auth/save-score
Content-Type: application/json

{
  "GridSize": 4,
  "Score": 2048
}
```

### Get Leaderboard
```http
GET /api/auth/leaderboard?gridSize=4&top=10
```

### Register New User
```http
POST /api/auth/register
Content-Type: application/json

{
  "Username": "newplayer",
  "Password": "password123"
}
```

## Ưu điểm của JSON Storage

✅ **Đơn giản**: Không cần cài đặt database
✅ **Persistent**: Dữ liệu không mất khi restart server
✅ **Readable**: Dễ đọc và chỉnh sửa thủ công nếu cần
✅ **Portable**: Dễ backup và restore (copy file)
✅ **Zero Config**: Không cần connection string

## Nhược điểm

❌ **Concurrency**: Không tốt với nhiều requests đồng thời (đã có lock)
❌ **Scalability**: Không phù hợp với hàng ngàn users
❌ **Query**: Không thể query phức tạp như SQL
❌ **Security**: Password không được mã hóa (plaintext)

## Migration sang Database (tương lai)

Nếu cần scale lên, có thể migrate sang:

### SQL Server / PostgreSQL
```csharp
// EF Core migrations
public class User
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Username { get; set; }
    
    // ... other properties
}
```

### MongoDB (NoSQL)
```csharp
// MongoDB document
[BsonCollection("users")]
public class User
{
    [BsonId]
    public ObjectId Id { get; set; }
    
    public string Username { get; set; }
    // ... other properties
}
```

## Backup và Restore

### Backup
```bash
# Windows
copy Data\users.json Data\users.backup.json

# Linux/Mac
cp Data/users.json Data/users.backup.json
```

### Restore
```bash
# Windows
copy Data\users.backup.json Data\users.json

# Linux/Mac
cp Data/users.backup.json Data/users.json
```

## Testing

### Xóa tất cả data (reset)
```bash
# Windows
del Data\users.json

# Linux/Mac
rm Data/users.json
```

File sẽ được tự động tạo lại với empty array `[]` khi server khởi động.

## Best Practices

1. ✅ **Backup thường xuyên**: Copy file users.json định kỳ
2. ✅ **Git ignore**: Thêm `Data/users.json` vào `.gitignore` nếu có dữ liệu nhạy cảm
3. ✅ **Validate input**: Luôn validate username/password trước khi save
4. ✅ **Error handling**: Catch exceptions khi đọc/ghi file
5. ✅ **Thread safety**: Service đã implement lock để tránh race conditions

## Troubleshooting

### File không tồn tại
- Service tự động tạo file rỗng khi khởi động
- Check quyền ghi trong thư mục Data/

### JSON format error
- Validate JSON với https://jsonlint.com/
- Restore từ backup nếu file bị corrupt

### Dữ liệu không cập nhật
- Check console logs: `[DATA] Saved X users`
- Verify file permissions
- Check lock conflicts (nếu có nhiều instances)

## Logs

Service ghi logs khi:
- Load users: `[DATA] Loaded X users from users.json`
- Save users: `[DATA] Saved X users to users.json`
- Update score: `[DATA] Updated high score for {username}: {gridSize}x{gridSize} = {score}`
- Errors: `[ERROR] Failed to load/save users: {message}`
