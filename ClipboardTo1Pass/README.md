# Clipboard to 1Password Tool

A Windows utility that copies clipboard content to a 1Password secure note with a global keyboard shortcut.

## Features

- **Global Hotkey**: Press `Ctrl+Shift+V` to instantly save clipboard content to 1Password
- **System Tray Integration**: Right-click the tray icon for options or double-click to save clipboard
- **Automatic Timestamping**: Each clipboard entry is timestamped when saved
- **Appends Content**: New clipboard content is appended to existing notes (doesn't overwrite)
- **Background Operation**: Runs silently in the background

## Prerequisites

1. **1Password CLI**: Must be installed and in your system PATH
   - Download from: https://developer.1password.com/docs/cli/get-started/
   - Verify installation: `op --version`

2. **1Password CLI Authentication**: Must be connected to your 1Password app
   - Run: `op signin` or ensure 1Password desktop app is running and CLI is connected

3. **Secure Note Setup**: Create a secure note in 1Password with the title `_CP`
   - This will be the target note where clipboard content is saved
   - You can verify it exists with: `op item get "_CP"`

4. **.NET 9 Runtime**: Must be installed
   - Download from: https://dotnet.microsoft.com/download/dotnet/9.0

## Installation

1. Clone or download this repository
2. Open a terminal/PowerShell in the project directory
3. Build the application:
   ```powershell
   cd ClipboardTo1Pass
   dotnet build -c Release
   ```

## Usage

### Running the Application

```powershell
cd ClipboardTo1Pass
dotnet run
```

Or run the built executable:
```powershell
.\bin\Debug\net9.0-windows\ClipboardTo1Pass.exe
```

### Using the Tool

1. **Start the application** - it will run in the background with a system tray icon
2. **Copy any text to clipboard** (Ctrl+C)
3. **Press `Ctrl+Shift+V`** to save the clipboard content to your 1Password secure note
4. **Check notifications** - the system tray will show success/error notifications

### System Tray Options

- **Double-click tray icon**: Save current clipboard content
- **Right-click tray icon**: 
  - "Save Clipboard Now" - Manual save
  - "Exit" - Close the application

## How It Works

1. The application registers a global hotkey (`Ctrl+Shift+V`) with Windows
2. When the hotkey is pressed:
   - Current clipboard text content is captured
   - The existing secure note `_CP` is retrieved from 1Password
   - New content is appended with a timestamp
   - The note is updated via 1Password CLI

## Security Notes

- Clipboard content is only stored in 1Password (not logged locally)
- Uses 1Password CLI for secure communication
- Requires existing 1Password authentication
- Only processes text clipboard content (ignores images, files, etc.)

## Troubleshooting

### "Failed to register hotkey"
- Another application might be using `Ctrl+Shift+V`
- Try closing other clipboard managers or tools
- Restart the application with administrator privileges

### "1Password CLI error"
- Verify 1Password CLI is installed: `op --version`
- Ensure you're signed in: `op whoami`
- Check the secure note exists: `op item get "_CP"`

### No system tray icon
- Check Windows notification area settings
- Ensure the application is running (check Task Manager)

## Building for Distribution

To create a standalone executable:

```powershell
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

This creates a single executable file that doesn't require .NET to be installed on the target machine.

## Customization

You can modify the following in `Program.cs`:

- **Hotkey combination**: Change `MOD_CONTROL | MOD_SHIFT` and `VK_V` constants
- **1Password item name**: Change `OnePasswordItemName` constant from `"_CP"`
- **Timestamp format**: Modify the `timestamp` format string

## License

This project is provided as-is for educational and personal use.
