# 🎮 HƯỚNG DẪN DEMO GAME 2048

## 🚀 Cách chạy ứng dụng

### 1. Khởi động server
```bash
cd "c:\Users\Quang\Desktop\CNTT\MTK"
dotnet run
```

Server sẽ chạy tại: **http://localhost:5000**

### 2. Mở trình duyệt
Truy cập: `http://localhost:5000`

---

## 👥 Tài khoản Demo

| Username | Password | Mô tả |
|----------|----------|-------|
| player1  | pass123  | Người chơi 1 |
| player2  | pass456  | Người chơi 2 |

---

## 🎯 Các Chế Độ Grid (Bảng chơi)

### 📊 3×3 Grid - Easy Mode
- **Kích thước**: 3×3 (9 ô)
- **Mục tiêu**: Đạt ô **128**
- **Độ khó**: Dễ - Nhanh chóng
- **Phù hợp**: Người mới bắt đầu

### 📊 4×4 Grid - Classic Mode
- **Kích thước**: 4×4 (16 ô)
- **Mục tiêu**: Đạt ô **2048**
- **Độ khó**: Trung bình - Chuẩn
- **Phù hợp**: Trải nghiệm game gốc

### 📊 5×5 Grid - Expert Mode
- **Kích thước**: 5×5 (25 ô)
- **Mục tiêu**: Đạt ô **4096**
- **Độ khó**: Khó - Thách thức
- **Phù hợp**: Người chơi giỏi

---

## ⚡ Các Strategy Pattern (Thuật toán di chuyển)

### 🎯 Standard Mode (Chế độ Chuẩn)
**Thuật toán**: `StandardMoveStrategy`

**Cách hoạt động**:
- Di chuyển các ô theo hướng được chọn
- Merge (gộp) 2 ô giống nhau thành 1 ô gấp đôi giá trị
- **CÓ TÍNH ĐIỂM**: Mỗi lần merge được cộng điểm = giá trị ô mới
- Ví dụ: 2+2 = 4 (+4 điểm), 4+4 = 8 (+8 điểm)

**Khi nào dùng**: 
- ✅ Chơi bình thường
- ✅ Muốn điểm số chính xác theo luật chuẩn

---

### ⚡ Fast Mode (Chế độ Nhanh)
**Thuật toán**: `FastMoveStrategy`

**Cách hoạt động**:
- Di chuyển và merge **2 LẦN** trong 1 lượt
- Cho phép merge liên tiếp trong cùng 1 nước đi
- **CÓ TÍNH ĐIỂM**: Điểm tăng nhanh hơn do merge nhiều lần
- Ví dụ: 2+2=4, sau đó 4+4=8 trong cùng 1 lần di chuyển

**Khi nào dùng**:
- ✅ Muốn điểm cao nhanh hơn
- ✅ Thử thách với gameplay khác

**Lưu ý**: Điểm số sẽ cao hơn chế độ Standard!

---

### 🧪 Test Mode (Chế độ Kiểm thử)
**Thuật toán**: `TestMoveStrategy`

**Cách hoạt động**:
- CHỈ di chuyển các ô, **KHÔNG merge**
- Các ô chỉ dồn về hướng được chọn mà không gộp lại
- **KHÔNG TÍNH ĐIỂM**: Score luôn = 0

**Khi nào dùng**:
- ✅ Kiểm thử logic di chuyển
- ✅ Xem cách ô di chuyển mà không merge
- ✅ Demo Design Pattern: Strategy

**Lưu ý**: Đây là chế độ test, không dùng để chơi thật!

---

## 🎲 Ví dụ Cụ Thể

### Trường hợp 1: Standard Mode
```
Trước:  [2][2][0][0]
        [0][0][0][0]
        [0][0][0][0]
        [0][0][0][0]

Di chuyển LEFT:
Sau:    [4][0][0][0]  ← 2+2=4, +4 điểm
        [2][0][0][0]  ← Ô mới random
        [0][0][0][0]
        [0][0][0][0]

Score: 4
```

### Trường hợp 2: Fast Mode
```
Trước:  [2][2][4][4]
        [0][0][0][0]
        [0][0][0][0]
        [0][0][0][0]

Di chuyển LEFT:
Bước 1: [4][8][0][0]  ← Merge lần 1: 2+2=4, 4+4=8
Bước 2: [4][8][0][0]  ← Merge lần 2 (không merge thêm)
Sau:    [4][8][0][0]
        [0][0][2][0]  ← Ô mới random

Score: 4 + 8 = 12 (cao hơn Standard!)
```

### Trường hợp 3: Test Mode
```
Trước:  [2][2][4][4]
        [0][0][0][0]
        [0][0][0][0]
        [0][0][0][0]

Di chuyển LEFT:
Sau:    [2][2][4][4]  ← CHỈ dồn, KHÔNG merge
        [2][0][0][0]  ← Ô mới random
        [0][0][0][0]
        [0][0][0][0]

Score: 0 (không tính điểm)
```

---

## 🎨 Design Patterns Demo

### 1️⃣ Singleton Pattern
**Class**: `GameManagerSingleton`
**Vị trí**: `Models/GameModels.cs`

**Minh họa**:
- Chỉ có 1 instance duy nhất quản lý game
- Lưu trữ: Board, Score, GridSize, Strategy
- Thread-safe implementation

### 2️⃣ Decorator Pattern
**Classes**: `ICell`, `CellDecorator`, `SpecialEffectDecorator`...
**Vị trí**: `Models/GameModels.cs` & `wwwroot/js/app.js`

**Minh họa**:
- **NewTileDecorator**: Animation pop-in cho ô mới
- **MergedTileDecorator**: Animation pulse khi merge
- **SpecialEffectDecorator**: Glow effect cho ô >= 512, Trophy 🏆 cho ô >= 1024

### 3️⃣ Strategy Pattern
**Interface**: `IMoveStrategy`
**Implementations**: 
- `StandardMoveStrategy`
- `FastMoveStrategy`
- `TestMoveStrategy`

**Minh họa**:
- Thay đổi thuật toán runtime
- Cùng interface, khác behavior
- Dễ dàng thêm strategy mới

---

## 🎯 Luồng Chơi Game

1. **Đăng nhập** → Chọn player1 hoặc player2
2. **Chọn Strategy** → Standard / Fast / Test
3. **Chọn Grid Size** → 3×3 / 4×4 / 5×5
4. **Chơi game**:
   - Di chuyển: Arrow keys hoặc swipe
   - Merge các ô giống nhau
   - Tăng điểm (trừ Test mode)
5. **Thắng** → Đạt mục tiêu (128/2048/4096)
6. **Thua** → Không còn nước đi

---

## 🔍 Kiểm Tra Tính Năng

### ✅ Checklist Demo
- [ ] Đăng nhập thành công với player1/player2
- [ ] Hiển thị high score riêng cho mỗi user
- [ ] Chọn được 3 strategy (Standard/Fast/Test)
- [ ] Tạo được 3 grid sizes (3×3, 4×4, 5×5)
- [ ] Di chuyển bằng phím mũi tên
- [ ] Merge ô đúng (trừ Test mode)
- [ ] Tính điểm đúng:
  - [ ] Standard: Tính điểm chuẩn
  - [ ] Fast: Điểm cao hơn
  - [ ] Test: Score = 0
- [ ] Animation hoạt động (pop-in, pulse, glow)
- [ ] Game Over khi hết nước đi
- [ ] You Win khi đạt mục tiêu
- [ ] Lưu high score cho mỗi grid size

---

## 💡 Tips Demo

### Để test Standard vs Fast Mode:
1. Chơi 1 game với Standard → Ghi nhớ điểm
2. Chơi lại với Fast → So sánh điểm (Fast cao hơn)

### Để test Test Mode:
1. Chọn Test mode
2. Di chuyển → Thấy ô chỉ dịch chuyển, không merge
3. Score luôn = 0

### Để test các Grid Sizes:
1. 3×3 → Chơi nhanh, mục tiêu 128
2. 4×4 → Trải nghiệm chuẩn, mục tiêu 2048
3. 5×5 → Khó hơn, mục tiêu 4096

---

## 🐛 Troubleshooting

### Server không chạy?
```bash
dotnet clean
dotnet build
dotnet run
```

### Port bị chiếm?
```bash
# Thay đổi port trong launchSettings.json
# Hoặc dùng:
dotnet run --urls "http://localhost:5001"
```

### Không kết nối được API?
- Kiểm tra Console (F12) xem có lỗi CORS không
- Đảm bảo server đang chạy

---

## 📚 Tài liệu Tham Khảo

- **Singleton Pattern**: `Models/GameModels.cs` dòng 8-47
- **Decorator Pattern**: `Models/GameModels.cs` dòng 165-260
- **Strategy Pattern**: `Models/GameModels.cs` dòng 270-544
- **API Endpoints**: `Controllers/GameController.cs`
- **Frontend Logic**: `wwwroot/js/app.js`

---

**Chúc bạn demo thành công!** 🎉
