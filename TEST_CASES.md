# 🧪 TEST CASES - Kiểm Tra Các Chế Độ

## Test Case 1: Standard Mode (3x3)

### Input:
- Grid Size: 3x3
- Strategy: Standard
- Board ban đầu có 2 ô random

### Các bước test:
1. Đăng nhập: player1/pass123
2. Chọn Strategy: Standard Mode
3. Chọn Grid: 3×3 Grid - Easy
4. Di chuyển: Phím mũi tên

### Expected Results:
✅ Grid 3x3 hiển thị đúng (9 ô)
✅ 2 ô ban đầu có giá trị 2 hoặc 4
✅ Mỗi lần di chuyển:
   - Các ô dịch chuyển đúng hướng
   - Ô giống nhau merge lại (2+2=4, 4+4=8...)
   - Điểm tăng = giá trị ô mới (merge 2+2=4 → +4 điểm)
   - 1 ô mới xuất hiện random
✅ Mục tiêu: Đạt 128 → You Win
✅ Không còn nước đi → Game Over

---

## Test Case 2: Standard Mode (4x4)

### Input:
- Grid Size: 4x4
- Strategy: Standard
- Board ban đầu có 2 ô random

### Các bước test:
1. Đăng nhập: player1/pass123
2. Chọn Strategy: Standard Mode
3. Chọn Grid: 4×4 Grid - Classic
4. Di chuyển: Phím mũi tên

### Expected Results:
✅ Grid 4x4 hiển thị đúng (16 ô)
✅ Merge và tính điểm đúng
✅ Mục tiêu: Đạt 2048 → You Win
✅ High score lưu riêng cho 4x4

---

## Test Case 3: Standard Mode (5x5)

### Input:
- Grid Size: 5x5
- Strategy: Standard

### Expected Results:
✅ Grid 5x5 hiển thị đúng (25 ô)
✅ Mục tiêu: Đạt 4096 → You Win
✅ Khó hơn do nhiều ô hơn

---

## Test Case 4: Fast Mode (4x4)

### Input:
- Grid Size: 4x4
- Strategy: Fast Mode

### Setup Test:
Tạo board có thể merge nhiều lần:
```
[2][2][4][4]
[0][0][0][0]
[0][0][0][0]
[0][0][0][0]
```

### Các bước test:
1. Đăng nhập: player2/pass456
2. Chọn Strategy: Fast Mode
3. Chọn Grid: 4×4
4. Di chuyển LEFT

### Expected Results:
✅ Lần merge 1: [2+2=4] [4+4=8] → [4][8][0][0]
✅ Lần merge 2: Có thể merge tiếp nếu có ô giống nhau
✅ Điểm tăng NHANH hơn Standard
✅ Ví dụ: merge chuỗi 2→4→8 trong 1 lượt

### So sánh với Standard:
- **Standard**: 2+2=4 (+4 điểm) XONG
- **Fast**: 2+2=4 (+4), rồi 4+4=8 (+8) = +12 điểm total

---

## Test Case 5: Test Mode (4x4)

### Input:
- Grid Size: 4x4
- Strategy: Test Mode

### Setup Test:
```
[2][2][4][4]
[8][8][0][0]
[0][0][0][0]
[0][0][0][0]
```

### Các bước test:
1. Chọn Strategy: Test Mode
2. Chọn Grid: 4×4
3. Di chuyển UP/DOWN/LEFT/RIGHT

### Expected Results:
✅ Các ô CHỈ dịch chuyển, KHÔNG merge
✅ Score = 0 (không tăng)
✅ Ví dụ di chuyển LEFT:
   - Trước: [2][2][4][4]
   - Sau:  [2][2][4][4] (dồn sang trái nhưng không gộp)

❌ KHÔNG được:
- Merge 2+2=4
- Tăng điểm
- Game Over (vì không merge nên luôn có chỗ)

### Mục đích:
🧪 Để test logic di chuyển mà không merge
🧪 Demo Strategy Pattern rõ ràng

---

## Test Case 6: Decorator Pattern - Animations

### Test animations cho các ô:

#### New Tile Animation:
1. Di chuyển → Ô mới xuất hiện
2. Expected: Animation pop-in (scale từ 0 → 1.1 → 1)

#### Merged Tile Animation:
1. Merge 2 ô (2+2=4)
2. Expected: Animation pulse (scale 1 → 1.15 → 1)

#### Special Glow Effect:
1. Tạo ô >= 512 (512, 1024, 2048...)
2. Expected: Box-shadow glow vàng, chớp chớp

#### Trophy Decorator:
1. Tạo ô >= 1024
2. Expected: Hiển thị icon 🏆 trước số

---

## Test Case 7: Multi-User High Scores

### Setup:
2 users: player1 & player2

### Các bước test:

#### User 1 - player1:
1. Login: player1/pass123
2. Chơi 3×3, đạt 100 điểm
3. Chơi 4×4, đạt 500 điểm
4. Logout

#### User 2 - player2:
1. Login: player2/pass456
2. Kiểm tra high scores → Phải là 0 (không thấy điểm của player1)
3. Chơi 3×3, đạt 150 điểm
4. Chơi 4×4, đạt 600 điểm
5. Logout

#### Verify User 1:
1. Login lại: player1/pass123
2. Kiểm tra high scores:
   - 3×3: 100 (điểm của player1)
   - 4×4: 500 (điểm của player1)
   - KHÔNG thấy điểm của player2

### Expected Results:
✅ High scores lưu riêng cho mỗi user
✅ High scores lưu riêng cho mỗi grid size
✅ Điểm cao nhất được cập nhật khi chơi lại

---

## Test Case 8: Responsive Design

### Desktop (> 768px):
✅ Mode cards hiển thị ngang (3 cards cạnh nhau)
✅ Grid size lớn, dễ nhìn
✅ Strategy buttons hiển thị ngang

### Tablet (768px):
✅ Mode cards xếp dọc
✅ Grid size tự động điều chỉnh

### Mobile (< 480px):
✅ Mode cards full width
✅ Grid nhỏ hơn nhưng vẫn chơi được
✅ Touch swipe hoạt động tốt
✅ Strategy buttons xếp dọc

---

## Test Case 9: Game State Management (Singleton)

### Test Singleton Pattern:

1. Bắt đầu game 3×3
2. Di chuyển vài lượt → Score = 50
3. Không reload page
4. Chuyển sang 4×4
5. Kiểm tra: Score phải reset về 0, board mới

### Expected:
✅ Mỗi lần New Game → State được reset
✅ Singleton quản lý đúng state hiện tại
✅ Không bị lẫn lộn giữa các games

---

## Test Case 10: Edge Cases

### Edge 1: Full Board (không còn chỗ trống)
Setup: Fill toàn bộ board với số khác nhau
Expected: Game Over

### Edge 2: Win nhưng tiếp tục chơi
1. Đạt mục tiêu (128/2048/4096)
2. Nhấn "Continue Playing"
3. Expected: Tiếp tục chơi, không hiện overlay nữa

### Edge 3: Không có nước đi hợp lệ
Setup: Board đầy, không ô nào merge được
Expected: Game Over ngay lập tức

### Edge 4: Strategy change giữa game
1. Bắt đầu với Standard
2. Di chuyển vài lượt
3. Change Mode → Chọn Fast
4. Expected: Strategy mới được áp dụng cho các nước tiếp theo

---

## 📊 Bảng So Sánh Strategies

| Feature | Standard | Fast | Test |
|---------|----------|------|------|
| Di chuyển | ✅ | ✅ | ✅ |
| Merge | ✅ 1 lần | ✅ Nhiều lần | ❌ Không |
| Tính điểm | ✅ Chuẩn | ✅ Cao hơn | ❌ Score=0 |
| Ô mới | ✅ | ✅ | ✅ |
| Game Over | ✅ | ✅ | Hiếm (vì không merge) |
| Win condition | ✅ | ✅ | ❌ |
| Mục đích | Chơi thật | Chơi nhanh | Demo/Test |

---

## ✅ Checklist Hoàn Chỉnh

### Backend (C#):
- [x] Singleton Pattern implemented
- [x] Decorator Pattern implemented
- [x] Strategy Pattern implemented
- [x] 3 Move Strategies (Standard/Fast/Test)
- [x] Auth Controller (Login/Logout/SaveScore)
- [x] Game Controller (NewGame/Move/SetStrategy)
- [x] User-specific high scores
- [x] Grid-specific high scores

### Frontend (HTML/CSS/JS):
- [x] Login screen
- [x] Mode selection screen (3 cards)
- [x] Strategy selection (3 buttons)
- [x] Game screen with dynamic grid
- [x] Animations (new-tile, merged-tile, glow)
- [x] Keyboard controls
- [x] Touch controls
- [x] Game Over overlay
- [x] You Win overlay
- [x] Responsive design

### Game Logic:
- [x] 3×3 grid → target 128
- [x] 4×4 grid → target 2048
- [x] 5×5 grid → target 4096
- [x] Standard strategy tính điểm đúng
- [x] Fast strategy điểm cao hơn
- [x] Test strategy không tính điểm
- [x] High scores lưu đúng

---

**Tất cả test cases đã sẵn sàng để demo!** 🎯
