# 🔐 Clipboard to 1Password Tool

> A Windows utility that securely saves clipboard content to 1Password with a global hotkey

## 🎯 Quick Start

### ⚡ Express Setup
1. **Run setup script**: `powershell -File setup.ps1` (verifies prerequisites and offers Windows startup)
2. **Start the application**: Double-click `ClipboardTo1Pass.exe` or run `powershell -File run.ps1`
3. **Copy text and press**: `Ctrl+Alt+F12` to save to 1Password

### 📋 Prerequisites
- Windows 10/11
- .NET 9 Runtime
- 1Password CLI in PATH (`op` command available)
- 1Password CLI authenticated (`op signin`)
- 1Password secure note named `_CP` (auto-created if missing)

## 🚀 Features

| Feature | Description |
|---------|-------------|
| **Global Hotkey** | `Ctrl+Alt+F12` saves clipboard instantly |
| **System Tray** | Runs minimized with right-click menu |
| **Windows Startup** | Optional auto-start with Windows |

## 🎮 Usage

### Method 1: Double-click to Run
```
Double-click ClipboardTo1Pass.exe
```

### Method 2: Using PowerShell Script
```powershell
# From the project folder
.\run.ps1
```

### Method 3: From Command Line
```powershell
# Navigate to the executable location
.\ClipboardTo1Pass.exe
```

### Method 4: Development Mode
```powershell
# Run from source code
dotnet run --project ClipboardTo1Pass
```

## 🔧 How It Works

1. **Startup**: Application registers global hotkey (`Ctrl+Alt+F12`) and creates system tray icon
1. **Hotkey Press**: Captures current clipboard text content
1. **1Password Update**: Uses CLI to securely update the secure note named `_CP`
1. **Notification**: Shows success/error message in system tray

## ⚙️ Configuration

### Change Hotkey
Edit `Program.cs` constants:
```csharp
private const int MOD_CONTROL = 0x0002;  // Ctrl key
private const int MOD_ALT = 0x0001;      // Alt key  
private const int VK_F12 = 0x7B;         // F12 key
```

### Change Target Note
```csharp
private static readonly string OnePasswordItemName = "_CP";
```

## 🐛 Troubleshooting

### Common Issues

| Problem | Solution |
|---------|----------|
| Hotkey not working | Check if another app uses `Ctrl+Alt+F12` |
| No system tray icon | Check Windows notification area settings |
| 1Password CLI errors | Verify installation: `op --version` |
| Authentication issues | Run: `op signin` |
| Missing secure note | Note `_CP` will be auto-created on first use |
| Application won't start | Check .NET 9 runtime is installed |

### Debug Mode
Run from command line to see detailed output:
```powershell
.\ClipboardTo1Pass.exe
```

### Setup Verification
Run the setup script to check all prerequisites:
```powershell
.\setup.ps1
```
## 📄 License

MIT License - see [LICENSE](LICENSE) file for details.
