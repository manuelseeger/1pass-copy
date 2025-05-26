# 📥 Installation Guide

## Quick Install (5 minutes)

### Step 1: Prerequisites ✅
Ensure you have:
- [x] Windows 10/11
- [x] [.NET 9 Runtime](https://dotnet.microsoft.com/download/dotnet/9.0)
- [x] [1Password CLI](https://developer.1password.com/docs/cli/get-started/) (must be in PATH)
- [x] 1Password account with desktop app

### Step 2: Setup 1Password CLI 🔐
```powershell
# Verify 1Password CLI is installed and in PATH
op --version

# Sign in to your 1Password account
op signin

# Verify connection
op whoami
```

### Step 3: Run Setup Script 🚀
```powershell
# Run our automated setup script
powershell -File setup.ps1
```

The setup script will:
- ✅ Verify .NET 9 runtime is installed
- ✅ Verify 1Password CLI is available in PATH
- ✅ Offer to add the app to Windows startup folder
- ✅ Create a shortcut for easy access

### Step 4: Start the Application 🎮
```powershell
# Option 1: Double-click the executable
# ClipboardTo1Pass.exe

# Option 2: Use the launcher script
.\run.ps1

# Option 3: From command line
.\ClipboardTo1Pass.exe
### Step 5: Test It! 🧪
1. Copy any text: `Hello, 1Password!`
2. Press: `Ctrl+Alt+F12`
3. Check your 1Password secure note `_CP` (will be auto-created if it doesn't exist)
4. You should see your text with a timestamp!

## Alternative Installation Methods

### For Developers
```powershell
# Clone repository
git clone <repository-url>
cd 1pass-copy

# Run setup and verification
.\setup.ps1

# Build and test
dotnet build -c Release
dotnet run --project ClipboardTo1Pass
```

### Standalone Deployment
```powershell
# Create self-contained executable (no .NET runtime required)
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true

# Result: Single .exe file in bin\Release\net9.0-windows\win-x64\publish\
```

### Manual Installation
1. Download or build the executable
2. Place `ClipboardTo1Pass.exe` in your desired location
3. (Optional) Add to Windows Startup folder:
   - Windows + R → `shell:startup`
   - Create shortcut to the executable
4. Run the executable

## Verification Checklist

Before first use, verify:
- [ ] 1Password CLI responds to `op --version`
- [ ] You're signed in: `op whoami` shows your account
- [ ] Application starts without errors
- [ ] System tray icon appears when running
- [ ] Hotkey `Ctrl+Alt+F12` is not used by other apps
- [ ] Secure note `_CP` gets created automatically on first use

## Common Installation Issues

### ❌ ".NET runtime not found"
**Solution**: Install [.NET 9 Runtime](https://dotnet.microsoft.com/download/dotnet/9.0)

### ❌ "'op' is not recognized as a command"
**Solution**: 
1. Install [1Password CLI](https://developer.1password.com/docs/cli/get-started/)
2. Ensure it's added to your system PATH
3. Restart your terminal and try `op --version`

### ❌ "Not signed in to 1Password CLI" 
**Solution**: Run `op signin` and follow the prompts

### ❌ "Failed to register hotkey"
**Solution**: 
- Close other clipboard managers that might use `Ctrl+Alt+F12`
- Check for conflicting software (AutoHotkey scripts, etc.)
- You can modify the hotkey in `Program.cs` if needed

### ❌ "Access denied" or permission errors
**Solution**: 
- Run as regular user (administrator privileges not required)
- Check Windows Defender or antivirus exclusions
- Ensure the executable isn't blocked by security software

### ❌ Application starts but no system tray icon
**Solution**: 
- Check Windows notification area settings
- Look for hidden icons in the system tray
- Restart the application

## Getting Help

If you encounter issues:
1. **Run the setup script**: `.\setup.ps1` for automated diagnostics
2. **Check the main README**: See troubleshooting section in README.md
3. **Run with console output**: Launch `ClipboardTo1Pass.exe` from PowerShell to see error messages
4. **Verify all prerequisites**: Ensure .NET 9 and 1Password CLI are properly installed

---

**Ready to keep your clipboard secure! 🛡️**

*Need help? The setup script (`setup.ps1`) will guide you through the process and verify everything is working correctly.*
