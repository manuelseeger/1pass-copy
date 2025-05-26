# 🔐 Clipboard to 1Password Tool

> A Windows utility that securely saves clipboard content to 1Password with a keyboard shortcut

## 🎯 Quick Start

### ⚡ Express Setup
1. **Run setup verification**: `powershell -File setup.ps1`
2. **Start the application**: `powershell -File run.ps1`
3. **Copy text and press**: `Ctrl+Shift+V`

### 📋 Prerequisites
- Windows 10/11
- .NET 9 Runtime
- 1Password CLI installed and authenticated
- 1Password secure note named `_CP`

## 🚀 Features

| Feature | Description |
|---------|-------------|
| **Global Hotkey** | `Ctrl+Shift+V` saves clipboard instantly |
| **System Tray** | Runs minimized with right-click menu |
| **Auto-Timestamp** | Each entry gets date/time stamp |
| **Content Append** | New content is added, not replaced |
| **Secure Integration** | Uses official 1Password CLI |
| **Error Handling** | Clear notifications and error messages |

## 📁 Project Structure

```
📦 Clipboard to 1Password Tool
├── 🏃 run.ps1                    # Start the application (PowerShell)
├── 🏃 run.bat                    # Start the application (Batch)
├── ⚙️ setup.ps1                  # Setup verification (PowerShell)
├── ⚙️ setup.bat                  # Setup verification (Batch)
├── 🎬 demo.ps1                   # Demonstration script
├── 📋 test.bat                   # Test 1Password connectivity
├── 📖 PROJECT_OVERVIEW.md        # Detailed project documentation
│
├── 📂 ClipboardTo1Pass/          # Main application
│   ├── 🎯 ClipboardTo1Pass.exe   # Ready-to-run executable
│   ├── 💻 Program.cs             # Core application logic
│   ├── ⚙️ ClipboardTo1Pass.csproj # Project configuration
│   ├── 📋 app.manifest           # Windows application manifest
│   └── 📖 README.md              # User instructions
│
└── 📂 Test1PassCLI/              # Connectivity test utility
    ├── 💻 Program.cs             # Test script
    └── ⚙️ Test1PassCLI.csproj    # Test project configuration
```

## 🎮 Usage

### Method 1: Direct Execution
```powershell
# Navigate to the executable
cd ClipboardTo1Pass\bin\Release\net9.0-windows\
.\ClipboardTo1Pass.exe
```

### Method 2: Using Scripts
```powershell
# PowerShell (Recommended)
.\run.ps1

# Command Prompt
run.bat
```

### Method 3: Development Mode
```powershell
# Run from source
dotnet run --project ClipboardTo1Pass
```

## 🔧 How It Works

1. **Startup**: Application registers global hotkey and creates system tray icon
2. **Hotkey Press**: `Ctrl+Shift+V` captures current clipboard text
3. **Content Retrieval**: Gets existing content from 1Password secure note `_CP`
4. **Content Append**: Adds timestamp and new clipboard content
5. **1Password Update**: Uses CLI to update the secure note
6. **Notification**: Shows success/error message in system tray

## ⚙️ Configuration

### Change Hotkey
Edit `Program.cs` constants:
```csharp
private const int MOD_CONTROL = 0x0002;  // Ctrl key
private const int MOD_SHIFT = 0x0004;    // Shift key  
private const int VK_V = 0x56;           // V key
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
dotnet build --project ClipboardTo1Pass

# Release build (recommended)
dotnet build --project ClipboardTo1Pass -c Release

# Create standalone executable
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

### VS Code Integration
- **Tasks**: Build, Build Release, Build and Run
- **Debug**: F5 to start debugging
- **IntelliSense**: Full C# support

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
| Hotkey not working | Check if another app uses `Ctrl+Shift+V` |
| No system tray icon | Check Windows notification area settings |
| 1Password CLI errors | Verify installation: `op --version` |
| Authentication issues | Run: `op signin` |
| Missing secure note | Create note named `_CP` in 1Password |

### Debug Mode
Run from command line to see detailed output:
```powershell
cd ClipboardTo1Pass
dotnet run
```

## 📦 Distribution

### For End Users
1. Copy the entire `bin\Release\net9.0-windows\` folder
2. Ensure .NET 9 runtime is installed
3. Run `ClipboardTo1Pass.exe`

### For Developers
1. Clone the repository
2. Run `setup.ps1` to verify prerequisites
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
   - Press `Ctrl+Shift+V`
   - See notification: "Clipboard content saved to 1Password!"

4. **Check 1Password**
   - Open secure note `_CP`
   - See timestamped entry with your content

## 🔮 Future Enhancements

- [ ] Configurable hotkeys via settings file
- [ ] Multiple target secure notes
- [ ] Content filtering and formatting
- [ ] Clipboard history management
- [ ] Auto-cleanup of old entries
- [ ] MSI installer package
- [ ] Portable version (no installation required)

## 📄 License

This project is provided as-is for educational and personal use.

---

**Built with ❤️ for secure clipboard management**

🛡️ *Keep your sensitive data safe with 1Password integration*
