# 📥 Installation Guide

## Quick Install (5 minutes)

### Step 1: Prerequisites ✅
Ensure you have:
- [x] Windows 10/11
- [x] [.NET 9 Runtime](https://dotnet.microsoft.com/download/dotnet/9.0)
- [x] [1Password CLI](https://developer.1password.com/docs/cli/get-started/)
- [x] 1Password account with desktop app

### Step 2: Setup 1Password CLI 🔐
```powershell
# Install 1Password CLI (if not already installed)
# Download from: https://developer.1password.com/docs/cli/get-started/

# Sign in to your 1Password account
op signin

# Verify connection
op whoami
```

### Step 3: Create Target Secure Note 📝
**Option A: Automatic (Recommended)**
```powershell
# Run our setup script
powershell -File setup.ps1
```

**Option B: Manual**
1. Open 1Password app
2. Create new Secure Note
3. Title: `_CP` (exactly as shown)
4. Add any initial content (optional)
5. Save

### Step 4: Download & Run 🚀
1. **Download** the latest release or clone this repository
2. **Navigate** to the project folder
3. **Run** the application:
   ```powershell
   # Option 1: Use our run script
   powershell -File run.ps1
   
   # Option 2: Run executable directly
   .\ClipboardTo1Pass\bin\Release\net9.0-windows\ClipboardTo1Pass.exe
   
   # Option 3: Run from source (developers)
   dotnet run --project ClipboardTo1Pass
   ```

### Step 5: Test It! 🧪
1. Copy any text: `Hello, 1Password!`
2. Press: `Ctrl+Shift+V`
3. Check your 1Password secure note `_CP`
4. You should see your text with a timestamp!

## Advanced Installation

### For System-Wide Use
1. Copy executable to `Program Files`
2. Add to Windows Startup folder
3. Create desktop shortcut

### For Development
```powershell
# Clone repository
git clone <repository-url>
cd clipboard-to-1password

# Verify setup
powershell -File setup.ps1

# Build and test
dotnet build -c Release
dotnet run --project ClipboardTo1Pass
```

### Standalone Deployment
```powershell
# Create self-contained executable
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true

# Result: Single .exe file that doesn't require .NET installation
```

## Verification Checklist

Before first use, verify:
- [ ] 1Password CLI responds to `op --version`
- [ ] You're signed in: `op whoami` shows your account
- [ ] Secure note `_CP` exists: `op item get "_CP"`
- [ ] Application builds without errors
- [ ] System tray icon appears when running
- [ ] Hotkey `Ctrl+Shift+V` is not used by other apps

## Common Installation Issues

### ❌ ".NET is not installed"
**Solution**: Install [.NET 9 Runtime](https://dotnet.microsoft.com/download/dotnet/9.0)

### ❌ "1Password CLI not found"
**Solution**: Install from [1Password CLI downloads](https://developer.1password.com/docs/cli/get-started/)

### ❌ "Not signed in to 1Password CLI"
**Solution**: Run `op signin` and follow prompts

### ❌ "Secure note '_CP' not found"
**Solution**: Create secure note manually or run `setup.ps1`

### ❌ "Failed to register hotkey"
**Solution**: Close other clipboard managers or change hotkey in code

### ❌ "Access denied" or permission errors
**Solution**: Run as administrator or check antivirus settings

## Support

If you encounter issues:
1. Run `powershell -File setup.ps1` for diagnostics
2. Check the troubleshooting section in README.md
3. Run the app from command line to see error messages
4. Verify all prerequisites are properly installed

---

**Ready to keep your clipboard secure! 🛡️**
