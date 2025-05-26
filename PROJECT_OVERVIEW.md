# Clipboard to 1Password Tool

## 🎯 Project Overview

A Windows desktop utility that automatically saves clipboard content to a 1Password secure note with a simple keyboard shortcut. Built with C# .NET 9 and designed to run silently in the background.

## ✨ Features

- **Global Hotkey**: `Ctrl+Shift+V` instantly saves clipboard content
- **System Tray Integration**: Runs minimized with right-click menu
- **Automatic Timestamping**: Each clipboard entry is timestamped
- **Content Appending**: New content is appended (doesn't overwrite existing)
- **Secure Integration**: Uses official 1Password CLI for security
- **Error Handling**: Comprehensive error messages and notifications

## 🛠️ Technical Stack

- **Platform**: Windows 10/11
- **Framework**: .NET 9 (WinForms)
- **Language**: C# 
- **Dependencies**: 
  - System.Windows.Extensions
  - Newtonsoft.Json
  - 1Password CLI

## 📁 Project Structure

```
ClipboardTo1Pass/
├── Program.cs              # Main application logic
├── app.manifest           # Windows application manifest
├── ClipboardTo1Pass.csproj # Project configuration
└── README.md              # Detailed user instructions

Test1PassCLI/              # Connectivity test utility
├── Program.cs             # 1Password CLI verification
└── Test1PassCLI.csproj    # Test project configuration

run.bat                    # Quick start script
test.bat                   # Connectivity test script
.vscode/
├── tasks.json             # Build/run tasks
└── launch.json            # Debug configuration
```

## 🚀 Quick Start

### Prerequisites
1. **1Password CLI** installed and in PATH
2. **1Password Desktop** app running and authenticated
3. **Secure Note** named `_CP` in your 1Password account
4. **.NET 9 Runtime** installed

### Installation & Usage
1. **Test connectivity**: Run `test.bat` to verify setup
2. **Start application**: Run `run.bat` or execute the built `.exe`
3. **Use hotkey**: Copy text, then press `Ctrl+Shift+V` to save to 1Password

## 🔧 Development

### Build Commands
```powershell
# Debug build
dotnet build --project ClipboardTo1Pass

# Release build  
dotnet build --project ClipboardTo1Pass -c Release

# Run application
dotnet run --project ClipboardTo1Pass
```

### VS Code Tasks
- **Ctrl+Shift+P** → "Tasks: Run Task"
- Available tasks: Build, Build Release, Build and Run

## 🎛️ Customization Options

### Change Hotkey
Edit `Program.cs` constants:
```csharp
private const int MOD_CONTROL = 0x0002;  // Ctrl
private const int MOD_SHIFT = 0x0004;    // Shift  
private const int VK_V = 0x56;           // V key
```

### Change Target Note
Modify the secure note name:
```csharp
private static readonly string OnePasswordItemName = "_CP";
```

### Change Timestamp Format
Update timestamp formatting:
```csharp
var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
```

## 🔒 Security Considerations

- **No Local Storage**: Clipboard content is only stored in 1Password
- **CLI Authentication**: Uses existing 1Password CLI session
- **Memory Safety**: Clipboard content is not logged or cached locally
- **Temporary Files**: No sensitive data written to temp files

## 🐛 Troubleshooting

### Common Issues

**"Failed to register hotkey"**
- Another app is using `Ctrl+Shift+V`
- Try running as administrator
- Check for clipboard managers or screen capture tools

**"1Password CLI error"**
- Verify CLI installation: `op --version`
- Check authentication: `op whoami`
- Confirm secure note exists: `op item get "_CP"`

**"No system tray icon"**
- Check Windows notification area settings
- Verify app is running in Task Manager

### Debug Mode
Run from command line to see detailed output:
```powershell
cd ClipboardTo1Pass
dotnet run
```

## 📦 Deployment

### Standalone Executable
Create a self-contained executable:
```powershell
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

### System Integration
- Add to Windows Startup folder for auto-start
- Create desktop shortcut to executable
- Pin to taskbar for easy access

## 🧪 Testing

Run the connectivity test before first use:
```powershell
cd Test1PassCLI
dotnet run
```

Or use the batch file:
```cmd
test.bat
```

## 📋 TODO / Future Enhancements

- [ ] Configurable hotkey via settings file
- [ ] Multiple target secure notes
- [ ] Content filtering/formatting options
- [ ] Clipboard history management
- [ ] Auto-cleanup of old entries
- [ ] Installer package (MSI/Setup)

## 📄 License

This project is provided as-is for educational and personal use.

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

---

**Built with ❤️ for Windows users who want secure clipboard management.**
