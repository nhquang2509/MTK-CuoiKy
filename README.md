# Game 2048 - Design Patterns Demo

Ứng dụng web game 2048 được xây dựng với ASP.NET Core (C#) và minh họa 3 Design Patterns chính.

## 🎮 Tính năng

### Chức năng Game
- ✅ **Đăng nhập**: 2 tài khoản demo (player1/pass123, player2/pass456)
- ✅ **Chọn chế độ**: 3x3 (Easy), 4x4 (Classic), 5x5 (Expert)
- ✅ **Lưu điểm cao**: Lưu riêng cho từng chế độ và người chơi
- ✅ **Điều khiển**: Phím mũi tên hoặc vuốt (touch gestures)
- ✅ **Responsive**: Tự động điều chỉnh trên mọi thiết bị

### Design Patterns Được Áp Dụng

#### 1. 🔹 Singleton Pattern
**File**: `Models/GameModels.cs` - Class `GameManagerSingleton`

**Mục đích**: Đảm bảo chỉ có 1 instance duy nhất quản lý trạng thái game (board, score, moves)

```csharp
public sealed class GameManagerSingleton
{
    private static GameManagerSingleton? _instance;
    private static readonly object _lock = new object();
    
    private GameManagerSingleton() { }
    
    public static GameManagerSingleton Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                        _instance = new GameManagerSingleton();
                }
            }
            return _instance;
        }
    }
}
```

#### 2. 🎨 Decorator Pattern
**File**: `Models/GameModels.cs` - Classes: `ICell`, `Cell`, `CellDecorator`

**Mục đích**: Trang trí cells với các tính năng bổ sung (hiệu ứng, màu sắc, animations) mà không thay đổi class cơ bản

**Các Decorators**:
- `SpecialEffectDecorator`: Thêm hiệu ứng glow cho ô >= 512, trophy icon cho ô >= 1024
- `NewTileDecorator`: Thêm animation pop-in cho ô mới
- `MergedTileDecorator`: Thêm animation pulse khi merge

```csharp
// Base component
public interface ICell
{
    int Value { get; set; }
    string GetDisplay();
    string GetCssClass();
}

// Decorator
public class SpecialEffectDecorator : CellDecorator
{
    public override string GetCssClass()
    {
        if (Value >= 512)
            return base.GetCssClass() + " special-glow";
        return base.GetCssClass();
    }
}
```

#### 3. ⚡ Strategy Pattern
**File**: `Models/GameModels.cs` - Interface `IMoveStrategy`

**Mục đích**: Thay đổi thuật toán di chuyển và merge mà không ảnh hưởng code chính

**Các Strategies**:
- `StandardMoveStrategy`: Thuật toán chuẩn (merge 1 lần)
- `FastMoveStrategy`: Merge nhiều lần trong 1 lượt
- `TestMoveStrategy`: Chỉ di chuyển, không merge (để test)

```csharp
public interface IMoveStrategy
{
    (int[,] newBoard, int scoreDelta) ExecuteMove(
        int[,] board, int gridSize, Direction direction);
}

// Sử dụng
_gameManager.MoveStrategy = new FastMoveStrategy();
```

## 📁 Cấu trúc Project

```
MTK/
├── Controllers/
│   ├── AuthController.cs      # Login, logout, save score
│   └── GameController.cs      # Game logic, move handling
├── Models/
│   ├── GameModels.cs          # Design Patterns implementation
│   └── UserModels.cs          # User, GameState models
├── wwwroot/
│   ├── css/
│   │   └── style.css          # Responsive styling với animations
│   ├── js/
│   │   └── app.js             # Frontend game controller
│   └── index.html             # Main UI (Login, Mode, Game screens)
├── Program.cs                 # ASP.NET Core configuration
└── Game2048.csproj
```

## 🚀 Hướng dẫn chạy

### Yêu cầu
- .NET 7.0 SDK hoặc mới hơn
- Web browser hiện đại (Chrome, Firefox, Edge)

### ⭐ CÁCH 1: Tự động (Khuyến nghị)

#### Windows - Double Click:
```
1. Double-click file: start.bat
2. Đợi browser tự mở (5 giây)
3. Đăng nhập và chơi!
```

#### Windows - PowerShell:
```
Click phải start.ps1 → "Run with PowerShell"
```

#### Linux/Mac:
```bash
chmod +x start.sh
./start.sh
```

### 📝 CÁCH 2: Thủ công

1. **Mở terminal tại thư mục project**:
```bash
cd "c:\Users\Quang\Desktop\CNTT\MTK"
```

2. **Restore dependencies**:
```bash
dotnet restore
```

3. **Build project**:
```bash
dotnet build
```

4. **Chạy ứng dụng**:
```bash
dotnet run
```

5. **Mở trình duyệt** và truy cập:
```
http://localhost:5000
```

### 👥 Tài khoản demo
- **Player 1**: username: `player1`, password: `pass123`
- **Player 2**: username: `player2`, password: `pass456`

### 🐛 Troubleshooting

#### Lỗi: ".NET SDK not found"
**Fix:** Cài .NET SDK từ https://dotnet.microsoft.com/download

#### Lỗi: Port 5000 đã được sử dụng
**Fix:** 
```bash
# Windows
netstat -ano | findstr :5000
taskkill /PID <PID> /F

# Linux/Mac
lsof -ti:5000 | xargs kill -9
```

#### Lỗi đăng nhập: "Sai tên đăng nhập hoặc mật khẩu"
**Fix:** Xem file `FIX_LOGIN_ERROR.md` để biết chi tiết

## 🎯 Cách chơi

1. **Đăng nhập** với 1 trong 2 tài khoản demo
2. **Chọn Strategy** (Standard/Fast/Test) - minh họa Strategy Pattern
3. **Chọn chế độ** (3x3/4x4/5x5)
4. **Điều khiển**:
   - PC: Phím mũi tên ←↑→↓
   - Mobile: Vuốt theo hướng muốn di chuyển
5. **Mục tiêu**:
   - 3x3: Đạt 128
   - 4x4: Đạt 2048
   - 5x5: Đạt 4096

## 🎨 Giao diện

### Trang đăng nhập
- Form đăng nhập đơn giản
- Hiển thị 2 demo accounts
- Giới thiệu các Design Patterns được sử dụng

### Trang chọn chế độ
- 3 cards cho 3 grid sizes (3x3, 4x4, 5x5)
- Hiển thị high score riêng cho từng mode
- Chọn Strategy Pattern (Standard/Fast/Test)
- Hover effects và animations

### Trang game
- Grid động thay đổi theo mode
- Score và Best score real-time
- Animations: pop-in (new tile), pulse (merge), glow (high values)
- Overlays cho Game Over và You Win
- Hướng dẫn cách chơi

## 🛠️ API Endpoints

### Authentication
- `POST /api/auth/login` - Đăng nhập
- `POST /api/auth/logout` - Đăng xuất
- `GET /api/auth/current` - Lấy user hiện tại
- `POST /api/auth/save-score` - Lưu điểm cao

### Game
- `POST /api/game/new-game` - Tạo game mới
- `POST /api/game/move` - Di chuyển (up/down/left/right)
- `GET /api/game/state` - Lấy trạng thái game
- `POST /api/game/set-strategy` - Chọn strategy (standard/fast/test)

## 📚 Kiến thức minh họa

### Design Patterns
✅ **Singleton Pattern** - Quản lý state tập trung
✅ **Decorator Pattern** - Mở rộng tính năng linh hoạt
✅ **Strategy Pattern** - Thay đổi thuật toán runtime

### Web Development
✅ ASP.NET Core Web API
✅ RESTful API design
✅ Session management
✅ Responsive CSS (Flexbox, Grid)
✅ JavaScript ES6+
✅ Touch gestures support

### Best Practices
✅ Separation of concerns
✅ Clean code structure
✅ Comments chi tiết
✅ Error handling
✅ Responsive design

## 📝 Ghi chú

- Code được viết đơn giản, dễ hiểu với comments chi tiết
- Tập trung vào việc minh họa Design Patterns rõ ràng
- UI/UX thân thiện, mượt mà với animations
- Hỗ trợ đầy đủ keyboard và touch controls
- Lưu điểm cao riêng cho từng mode và user

## 🎓 Demo cho Assignment

Project này minh họa đầy đủ 3 Design Patterns được yêu cầu:

1. ✅ **Singleton**: GameManagerSingleton trong `Models/GameModels.cs`
2. ✅ **Decorator**: Cell decorators trong `Models/GameModels.cs`
3. ✅ **Strategy**: MoveStrategy trong `Models/GameModels.cs`

Mỗi pattern đều có comments chi tiết giải thích cách hoạt động và lý do sử dụng.
