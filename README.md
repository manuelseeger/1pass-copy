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
| **Auto-Timestamp** | Each entry gets date/time stamp |
| **Content Append** | New content is added, not replaced |
| **Secure Integration** | Uses official 1Password CLI |
| **Windows Startup** | Optional auto-start with Windows |
| **Error Handling** | Clear notifications and error messages |

## 📁 Project Structure

```
📦 ClipboardTo1Pass/
├── 🚀 ClipboardTo1Pass.exe      # Main executable (ready to run)
├── ⚙️ setup.ps1                 # Setup script with Windows startup option
├── 🏃 run.ps1                   # Launcher script  
├── 💻 Program.cs                # Core application source code
├── ⚙️ ClipboardTo1Pass.csproj   # .NET project file
├── 📋 app.manifest              # Windows application manifest
├── 📖 README.md                 # This file
├── 📖 INSTALL.md                # Installation guide
└── 📄 LICENSE                   # MIT license
```

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
2. **Hotkey Press**: Captures current clipboard text content
3. **Content Retrieval**: Gets existing content from 1Password secure note `_CP`
4. **Content Append**: Adds timestamp and new clipboard content
5. **1Password Update**: Uses CLI to securely update the secure note
6. **Notification**: Shows success/error message in system tray

## ⚙️ Configuration

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

### Change Timestamp Format
```csharp
var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
```

## 🛠️ Development

### Build Commands
```powershell
# Debug build
dotnet build

# Release build (recommended)
dotnet build -c Release

# Create standalone executable
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

### Development Setup
```powershell
# Verify prerequisites
.\setup.ps1

# Build and run
dotnet run
```

## 🔒 Security

✅ **Secure by Design**
- No local storage of clipboard content
- Uses official 1Password CLI authentication
- Temporary files are cleaned up automatically
- No sensitive data in logs or memory dumps

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

## 📦 Distribution

### For End Users
1. Download the latest release or executable
2. Ensure .NET 9 runtime is installed
3. Run `setup.ps1` to verify prerequisites
4. Double-click `ClipboardTo1Pass.exe` to start

### For Developers
1. Clone the repository
2. Run `setup.ps1` to verify prerequisites and setup Windows startup
3. Build with `dotnet build -c Release`
4. Customize as needed

## 🎯 Example Workflow

1. **Start the application**
   ```powershell
   .\ClipboardTo1Pass.exe
   ```

2. **Copy sensitive information**
   ```
   Username: john.doe@company.com
   Password: SuperSecretPassword123!
   Server: production-db.company.com
   ```

3. **Save to 1Password**
   - Press `Ctrl+Alt+F12`
   - See notification: "Clipboard content saved to 1Password!"

4. **Check 1Password**
   - Open secure note `_CP`
   - See timestamped entry with your content

## 📄 License

MIT License - see [LICENSE](LICENSE) file for details.

---

**Built with ❤️ for secure clipboard management**

🛡️ *Keep your sensitive data safe with 1Password integration*
