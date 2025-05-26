# Start Clipboard to 1Password Tool
# PowerShell script to run the application

Write-Host "Starting Clipboard to 1Password Tool..." -ForegroundColor Cyan
Write-Host ""
Write-Host "This tool will run in the background and register the hotkey Ctrl+Shift+V" -ForegroundColor Yellow
Write-Host "to save clipboard content to your 1Password secure note '_CP'." -ForegroundColor Yellow
Write-Host ""
Write-Host "Look for the system tray icon to confirm it's running." -ForegroundColor Green
Write-Host "Press Ctrl+C in this window to exit when done." -ForegroundColor White
Write-Host ""

# Change to the project directory
Set-Location "$PSScriptRoot\ClipboardTo1Pass"

# Run the application
try {
    dotnet run
}
catch {
    Write-Host "Error running the application: $($_.Exception.Message)" -ForegroundColor Red
    Read-Host "Press Enter to continue..."
}
