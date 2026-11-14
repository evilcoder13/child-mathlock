# 🚀 MathLock

**MathLock** is an interactive Windows desktop lock application built with C# and WPF (.NET 8) designed for elementary school pupils. Every time the user logs in or unlocks the computer, MathLock presents **10 quick, interactive mental math challenges** that must be solved before the desktop becomes usable.

---

## ✨ Features

### 🎮 Engaging & Elementary-Friendly Math
Instead of plain repetitive drills, MathLock mixes 5 interactive question styles tailored for elementary students:
1. **Quick Mental Math**: Clean addition, subtraction, multiplication, and division (`12 + 15 = ?`, `6 × 7 = ?`, `36 ÷ 4 = ?` — never negative or with messy remainders).
2. **Find the Missing Number**: Algebraic thinking for kids (`7 + ? = 15`, `? × 4 = 24`).
3. **Quick Compare**: Relational comparison (`3 × 6  [ ? ]  20 - 3` — choose `<`, `=`, or `>`).
4. **Number Bonds**: Mental complements (`Make 100! 65 + [ ? ] = 100`).
5. **Visual Groups**: Emoji-based group counting (`4 groups of 3 🚀 = ?`).

### ⏱️ Anti-Calculator Design
- **Speed Countdown Bar**: Each question has a configurable countdown timer (default **12 seconds**) — plenty of time for mental math, but too fast to grab a phone or calculator and punch in numbers.
- **No Penalty Reset**: If the timer expires, a fresh question immediately replaces it without advancing the star count.
- **10-Star Progress Tracker**: Every solved question lights up a gold star (`★`) with cheerful audio feedback until all 10 stars are earned.

### 🔒 Unclosable Multi-Monitor Lock
- **Fullscreen Kiosk Overlay**: Covers the primary screen with the math challenge and places blackout blocker overlays across all secondary monitors.
- **Low-Level Keyboard Hook (`WH_KEYBOARD_LL`)**: Blocks `Alt+F4`, `Alt+Tab`, `Alt+Esc`, `Ctrl+Esc`, and both `Windows` keys while the lock screen is active.
- **Focus Guard**: Automatically re-activates and stays topmost if another background process attempts to steal focus.

### 🛡️ Secret Parent Override
- **Hidden Corner Tap**: Click or tap the **top-left corner of the screen 5 times within 3 seconds** to open the **Parent Override PIN Dialog**.
- **Default Parent PIN**: `1234` (stored as a SHA-256 hash in `%LocalAppData%\MathLock\settings.json` and customizable in the Settings window).

---

## 🛠️ Prerequisites

- **OS**: Windows 10 or Windows 11
- **Runtime / SDK**: [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (with Windows Desktop runtime)

---

## 🚀 Quick Start

### 1. Build the Solution
```powershell
dotnet build MathLock.sln
```

### 2. Run Unit Tests
```powershell
dotnet test MathLock.Tests/MathLock.Tests.csproj
```

### 3. Launch the Math Lock Screen
```powershell
dotnet run --project MathLock/MathLock.csproj -- --lock
```

### 4. Open Parent Settings
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
- Change the Parent Override PIN.
- Enable or disable Windows Logon & Unlock auto-triggers.

---

## ⚙️ Command-Line Arguments

| Argument | Description |
| :--- | :--- |
| *(none)* or `--lock` | Launches the fullscreen MathLock challenge immediately. |
| `--settings` | Opens the Parent Configuration window. |
| `--tray` | Runs silently in the background and listens to Windows `SessionSwitch` events (`SessionLogon` and `SessionUnlock`) to pop up the lock screen automatically. |

---

## 🔄 Automatic Trigger on Windows Logon & Unlock

### Option A: Via the PowerShell Installer Script (Recommended)
1. Publish a release executable:
   ```powershell
   dotnet publish MathLock/MathLock.csproj -c Release -r win-x64 --self-contained false
   ```
2. Run the installer script in an **Administrator PowerShell** terminal:
   ```powershell
   powershell -ExecutionPolicy Bypass -File .\MathLock\install-task.ps1
   ```
   This registers two Windows Scheduled Tasks:
   - `MathLock_AutoLock_Logon` (triggers at user logon)
   - `MathLock_AutoLock_Unlock` (triggers on workstation unlock, state change `8`)

3. To remove the scheduled tasks at any time:
   ```powershell
   powershell -ExecutionPolicy Bypass -File .\MathLock\install-task.ps1 -Uninstall
   ```

### Option B: Via the Settings GUI
Run `MathLock.exe --settings` as Administrator and click **"Enable Auto-Lock on Logon & Unlock"**.

---

## 📂 Project Structure

```text
MathLock/
├── MathLock.sln
├── README.md
├── MathLock/
│   ├── MathLock.csproj              # .NET 8 WPF project
│   ├── app.manifest                 # Windows application manifest
│   ├── App.xaml / App.xaml.cs       # Single-instance mutex & CLI mode routing
│   ├── Models/
│   │   ├── MathQuestion.cs          # Question types, expressions, choices, visual hints
│   │   └── AppSettings.cs           # Difficulty presets, timer config, SHA-256 PIN verification
│   ├── Services/
│   │   ├── QuestionGenerator.cs     # Generates interactive elementary math questions & distractors
│   │   ├── SettingsService.cs       # Loads/saves settings in %LocalAppData%\MathLock\settings.json
│   │   ├── KeyboardBlocker.cs       # Win32 low-level keyboard hook (WH_KEYBOARD_LL)
│   │   ├── DisplayService.cs        # Win32 multi-monitor enumeration
│   │   ├── SoundService.cs          # Audio chimes for correct/wrong/victory states
│   │   └── TaskSchedulerService.cs  # Registers Windows Logon & Session Unlock tasks
│   ├── Views/
│   │   ├── LockWindow.xaml / .cs    # Primary fullscreen math challenge UI & 5-tap corner detector
│   │   ├── BlockerWindow.xaml / .cs # Secondary monitor blackout overlay
│   │   ├── ParentPinDialog.xaml/.cs # Parent PIN override modal with touch/mouse numpad
│   │   └── SettingsWindow.xaml/.cs  # Parent settings UI
│   └── install-task.ps1             # Scheduled Task installer/uninstaller script
└── MathLock.Tests/
    ├── MathLock.Tests.csproj        # xUnit test project
    ├── QuestionGeneratorTests.cs    # Unit tests for question generation & bounds
    └── AppSettingsTests.cs          # Unit tests for PIN hashing & validation
```

---

## 🔑 Parent Emergency Unlock Reminder

If you ever need to bypass the math screen immediately:
1. Click or tap the **top-left corner** of the primary screen **5 times within 3 seconds**.
2. Enter your Parent PIN (default: **`1234`**).
3. Click **Unlock Computer**.
