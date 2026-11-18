# 🚀 MathLock

🌐 **[English](#-english) | [Tiếng Việt](#-tiếng-việt)**

---

## 🇬🇧 English

**MathLock** is an interactive Windows desktop lock application built with C# and WPF (.NET 8) designed for elementary school pupils. Every time the user logs in, unlocks the computer, or after every 30 minutes of usage, MathLock presents **10 quick, interactive mental math challenges** that must be solved before the desktop becomes usable.

### ✨ Features

#### 🎮 Engaging & Elementary-Friendly Math
Instead of plain repetitive drills, MathLock mixes 5 interactive question styles tailored for elementary students:
1. **Quick Mental Math**: Clean addition, subtraction, multiplication, and division (`12 + 15 = ?`, `6 × 7 = ?`, `36 ÷ 4 = ?` — never negative or with messy remainders).
2. **Find the Missing Number**: Algebraic thinking for kids (`7 + ? = 15`, `? × 4 = 24`).
3. **Quick Compare**: Relational comparison (`3 × 6  [ ? ]  20 - 3` — choose `<`, `=`, or `>`).
4. **Number Bonds**: Mental complements (`Make 100! 65 + [ ? ] = 100`).
5. **Visual Groups**: Emoji-based group counting (`4 groups of 3 🚀 = ?`).

#### ⏱️ Anti-Calculator Design
- **Speed Countdown Bar**: Each question has a configurable countdown timer (default **12 seconds**) — plenty of time for mental math, but too fast to grab a phone or calculator and punch in numbers.
- **No Penalty Reset**: If the timer expires, a fresh question immediately replaces it without advancing the star count.
- **10-Star Progress Tracker**: Every solved question lights up a gold star (`★`) with cheerful audio feedback until all 10 stars are earned.

#### 🔒 Unclosable Multi-Monitor Lock
- **Fullscreen Kiosk Overlay**: Covers the primary screen with the math challenge and places blackout blocker overlays across all secondary monitors.
- **Low-Level Keyboard Hook (`WH_KEYBOARD_LL`)**: Blocks `Alt+F4`, `Alt+Tab`, `Alt+Esc`, `Ctrl+Esc`, and both `Windows` keys while the lock screen is active.
- **Focus Guard**: Automatically re-activates and stays topmost if another background process attempts to steal focus.

#### 🛡️ Secret Parent Override
- **Hidden Corner Tap**: Click or tap the **top-left corner of the screen 5 times within 3 seconds** to open the **Parent Override PIN Dialog**.
- **Default Parent PIN**: `1234` (stored as a SHA-256 hash in `%LocalAppData%\MathLock\settings.json` and customizable in the Settings window).

### 🛠️ Prerequisites

- **OS**: Windows 10 or Windows 11
- **Runtime / SDK**: [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (with Windows Desktop runtime)

### 🚀 Quick Start

#### 1. Build the Solution
```powershell
dotnet build MathLock.sln
```

#### 2. Run Unit Tests
```powershell
dotnet test MathLock.Tests/MathLock.Tests.csproj
```

#### 3. Launch the Math Lock Screen
```powershell
dotnet run --project MathLock/MathLock.csproj -- --lock
```

#### 4. Open Parent Settings
```powershell
dotnet run --project MathLock/MathLock.csproj -- --settings
```
From the Settings window, you can:
- Choose the difficulty tier:
  - **Grade 1–2**: Addition & subtraction within 20, visual counting, number bonds to 10.
  - **Grade 2–3 (Default)**: `+`, `-`, `×`, `÷` within 100, comparisons, missing operands, bonds to 100.
  - **Grade 4–5**: Multi-digit arithmetic and multiplication/division up to 12×12.
- Adjust the number of required questions (`3`–`25`, default `10`).
- Adjust the per-question anti-calculator timer (`5`–`30` seconds, default `12s`).
- Enable/disable periodic auto-lock and configure interval (`5`–`120` minutes, default **`30 minutes`**).
- Change the Parent Override PIN.
- Enable or disable Windows Logon & Unlock auto-triggers.

### ⚙️ Command-Line Arguments

| Argument | Description |
| :--- | :--- |
| *(none)* or `--lock` | Launches the fullscreen MathLock challenge immediately. |
| `--settings` | Opens the Parent Configuration window. |
| `--tray` | Runs silently in the background, enforces lock on user logon, listens to Windows `SessionSwitch` / WTS events (`SessionUnlock`), and runs the periodic 30-minute lock timer. |

### 🔄 Automatic Trigger on Windows Logon, Unlock & Every 30 Minutes

MathLock installs into `%LocalAppData%\Programs\MathLock` and registers with Windows Auto-Start (`HKCU\...\Run`) with `--tray`:
1. **At Windows Logon**: Starts automatically in the background and activates the math challenge immediately.
2. **At Workstation Unlock**: Listens to Windows `SessionUnlock` / `WM_WTSSESSION_CHANGE` events and locks the screen.
3. **Every 30 Minutes**: Triggers the 10 math challenges automatically while the computer is being used. The timer resets each time the child finishes solving the math.

To toggle or reconfigure:
- Open **Start Menu** -> search for **MathLock Settings**
- Or run `MathLock.exe --settings`

### 🔑 Parent Emergency Unlock Reminder
1. Click or tap the **top-left corner** of the primary screen **5 times within 3 seconds**.
2. Enter your Parent PIN (default: **`1234`**).
3. Click **Unlock Computer**.

---

## 🇻🇳 Tiếng Việt

**MathLock** là ứng dụng khóa màn hình máy tính tương tác trên Windows được viết bằng C# và WPF (.NET 8) dành riêng cho học sinh tiểu học. Mỗi khi người dùng đăng nhập (login), mở khóa máy (unlock), hoặc sau mỗi 30 phút sử dụng, MathLock sẽ hiển thị **10 câu đố toán tính nhẩm nhanh** buộc bé phải hoàn thành trước khi có thể sử dụng máy tính.

### ✨ Tính năng nổi bật

#### 🎮 Toán học tương tác, phù hợp học sinh tiểu học
Thay vì các phép tính khô khan lặp đi lặp lại, MathLock kết hợp 5 dạng câu hỏi sinh động:
1. **Tính nhẩm nhanh (Quick Mental Math)**: Cộng, trừ, nhân, chia nguyên chẵn (`12 + 15 = ?`, `6 × 7 = ?`, `36 ÷ 4 = ?` — không có số âm hay phép chia dư lẻ).
2. **Tìm số còn thiếu (Find the Missing Number)**: Rèn tư duy đại số sớm (`7 + ? = 15`, `? × 4 = 24`).
3. **So sánh nhanh (Quick Compare)**: So sánh hai vế biểu thức (`3 × 6  [ ? ]  20 - 3` — chọn `<`, `=`, hoặc `>`).
4. **Ghép số tròn chục / tròn trăm (Number Bonds)**: Tính nhẩm phần bù (`Make 100! 65 + [ ? ] = 100`).
5. **Đếm nhóm hình ảnh (Visual Groups)**: Đếm theo nhóm biểu tượng cảm xúc (`4 groups of 3 🚀 = ?`).

#### ⏱️ Thiết kế chống dùng máy tính cầm tay (Anti-Calculator)
- **Thanh đếm ngược tốc độ**: Mỗi câu hỏi có thời gian giới hạn (mặc định **12 giây**) — đủ thoải mái để tính nhẩm trong đầu, nhưng quá nhanh để bé kịp lấy điện thoại hoặc máy tính bỏ túi ra bấm.
- **Đổi câu mới khi hết giờ**: Nếu hết thời gian, hệ thống tự động đổi sang câu hỏi mới mà không tăng số sao.
- **Thanh tiến trình 10 Ngôi sao**: Mỗi câu trả lời đúng sẽ thắp sáng một ngôi sao vàng (`★`) kèm hiệu ứng âm thanh khích lệ cho đến khi đủ 10 sao.

#### 🔒 Khóa toàn màn hình đa màn hình (Multi-Monitor)
- **Giao diện Kiosk toàn màn hình**: Phủ kín màn hình chính bằng thử thách toán học và tự động che tối tất cả các màn hình phụ.
- **Chặn phím tắt hệ thống (`WH_KEYBOARD_LL`)**: Vô hiệu hóa `Alt+F4`, `Alt+Tab`, `Alt+Esc`, `Ctrl+Esc` và phím `Windows` trong khi màn hình khóa đang bật.
- **Tự động giữ tiêu điểm (Focus Guard)**: Tự động lấy lại quyền ưu tiên hiển thị trên cùng nếu có ứng dụng khác cố tình đè lên.

#### 🛡️ Cơ chế mở khóa khẩn cấp dành cho Phụ huynh
- **Chạm góc ẩn (Hidden Corner Tap)**: Click chuột hoặc chạm **5 lần liên tiếp vào góc trên cùng bên trái màn hình trong vòng 3 giây** để mở hộp thoại nhập mã PIN Phụ huynh.
- **Mã PIN mặc định**: `1234` (được băm bảo mật SHA-256 lưu tại `%LocalAppData%\MathLock\settings.json` và có thể đổi trong phần Cài đặt).

### 🛠️ Yêu cầu hệ thống

- **Hệ điều hành**: Windows 10 hoặc Windows 11
- **Runtime / SDK**: [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (kèm Windows Desktop runtime)

### 🚀 Hướng dẫn chạy nhanh

#### 1. Biên dịch mã nguồn (Build)
```powershell
dotnet build MathLock.sln
```

#### 2. Chạy kiểm thử tự động (Unit Tests)
```powershell
dotnet test MathLock.Tests/MathLock.Tests.csproj
```

#### 3. Chạy thử màn hình khóa toán học ngay lập tức
```powershell
dotnet run --project MathLock/MathLock.csproj -- --lock
```

#### 4. Mở bảng Cài đặt Phụ huynh (Settings)
```powershell
dotnet run --project MathLock/MathLock.csproj -- --settings
```
Trong cửa sổ Cài đặt, phụ huynh có thể:
- Chọn cấp độ khó phù hợp với lứa tuổi:
  - **Lớp 1–2 (Grade 1–2)**: Cộng & trừ trong phạm vi 20, đếm nhóm hình ảnh, ghép số tròn 10.
  - **Lớp 2–3 (Grade 2–3 - Mặc định)**: `+`, `-`, `×`, `÷` trong phạm vi 100, so sánh, tìm số ẩn, ghép số tròn 100.
  - **Lớp 4–5 (Grade 4–5)**: Phép tính nhiều chữ số và bảng nhân/chia tới 12×12.
- Điều chỉnh số lượng câu hỏi cần vượt qua (`3`–`25`, mặc định `10` câu).
- Điều chỉnh thời gian đếm ngược mỗi câu (`5`–`30` giây, mặc định `12s`).
- Bật/tắt tự động khóa định kỳ và chỉnh chu kỳ thời gian (`5`–`120` phút, mặc định **`30 phút`**).
- Đổi mã PIN Phụ huynh.
- Bật hoặc tắt chế độ tự khởi động cùng Windows.

### ⚙️ Tham số dòng lệnh (CLI Arguments)

| Tham số | Mô tả |
| :--- | :--- |
| *(trống)* hoặc `--lock` | Mở ngay lập tức màn hình khóa giải toán toàn màn hình. |
| `--settings` | Mở cửa sổ Cài đặt dành cho Phụ huynh. |
| `--tray` | Chạy ngầm trong hệ thống, tự khóa khi mới đăng nhập, lắng nghe sự kiện mở khóa máy (`SessionUnlock`), và đếm ngược chu kỳ khóa 30 phút/lần. |

### 🔄 Tự động kích hoạt khi Đăng nhập, Mở khóa & Mỗi 30 Phút

Khi cài đặt vào `%LocalAppData%\Programs\MathLock`, ứng dụng đăng ký tự khởi chạy cùng Windows (`HKCU\...\Run`) với tham số `--tray`:
1. **Khi đăng nhập Windows (Logon)**: Tự động chạy ngầm và bật ngay màn hình thử thách 10 câu toán.
2. **Khi mở khóa máy (Unlock)**: Lắng nghe sự kiện `SessionUnlock` / `WM_WTSSESSION_CHANGE` của Windows để khóa màn hình yêu cầu giải toán.
3. **Định kỳ mỗi 30 phút**: Tự động hiện thử thách 10 câu toán sau mỗi 30 phút sử dụng máy. Bộ đếm 30 phút sẽ được đặt lại từ đầu sau mỗi lần bé hoàn thành bài toán.

Để mở cài đặt cấu hình bất cứ lúc nào:
- Mở **Start Menu** -> tìm **MathLock Settings**
- Hoặc chạy lệnh `MathLock.exe --settings`

### 🔑 Nhắc nhở cách mở khóa nhanh cho Phụ huynh
Khi bạn cần dùng máy gấp mà không muốn làm 10 câu toán:
1. Click hoặc chạm **góc trên cùng bên trái màn hình chính 5 lần trong vòng 3 giây**.
2. Nhập mã PIN Phụ huynh (mặc định: **`1234`**).
3. Nhấn **Unlock Computer**.

---

## 📂 Project Structure / Cấu trúc thư mục

```text
MathLock/
├── MathLock.sln
├── README.md
├── MathLock/
│   ├── MathLock.csproj                      # .NET 8 WPF project
│   ├── app.manifest                         # Windows application manifest
│   ├── App.xaml / App.xaml.cs               # Single-instance mutex, tray timer & CLI mode routing
│   ├── Models/
│   │   ├── MathQuestion.cs                  # Question types, expressions, choices, visual hints
│   │   └── AppSettings.cs                   # Difficulty presets, timer config, SHA-256 PIN verification
│   ├── Services/
│   │   ├── QuestionGenerator.cs             # Generates interactive elementary math questions & distractors
│   │   ├── SettingsService.cs               # Loads/saves settings in %LocalAppData%\MathLock\settings.json
│   │   ├── KeyboardBlocker.cs               # Win32 low-level keyboard hook (WH_KEYBOARD_LL)
│   │   ├── DisplayService.cs                # Win32 multi-monitor enumeration
│   │   ├── SessionNotificationListener.cs   # Win32 WTS session lock/unlock notification listener
│   │   ├── StartupService.cs                # Windows Registry HKCU Run startup management
│   │   ├── SoundService.cs                  # Audio chimes for correct/wrong/victory states
│   │   ├── Logger.cs                        # Diagnostic event logger (%LocalAppData%\MathLock\app.log)
│   │   └── TaskSchedulerService.cs          # Registers Windows Logon & Session Unlock tasks
│   ├── Views/
│   │   ├── LockWindow.xaml / .cs            # Primary fullscreen math challenge UI & 5-tap corner detector
│   │   ├── BlockerWindow.xaml / .cs         # Secondary monitor blackout overlay
│   │   ├── ParentPinDialog.xaml / .cs       # Parent PIN override modal with touch/mouse numpad
│   │   └── SettingsWindow.xaml / .cs        # Parent settings UI
│   └── install-task.ps1                     # Scheduled Task installer/uninstaller script
└── MathLock.Tests/
    ├── MathLock.Tests.csproj                # xUnit test project
    ├── QuestionGeneratorTests.cs            # Unit tests for question generation & bounds
    └── AppSettingsTests.cs                  # Unit tests for PIN hashing & validation
```
