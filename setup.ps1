# Clipboard to 1Password Setup Verification
# PowerShell version

Write-Host "====================================" -ForegroundColor Cyan
Write-Host "Clipboard to 1Password Setup Wizard" -ForegroundColor Cyan
Write-Host "====================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "Checking prerequisites..." -ForegroundColor Yellow
Write-Host ""

# Function to check if a command exists
function Test-CommandExists {
    param($Command)
    try {
        $null = Get-Command $Command -ErrorAction Stop
        return $true
    }
    catch {
        return $false
    }
}

# Check if .NET 9 is installed
Write-Host "[1/2] Checking .NET 9 installation..." -ForegroundColor White
if (Test-CommandExists "dotnet") {
    try {
        $dotnetVersion = dotnet --version 2>$null
        Write-Host "✅ .NET is installed (version: $dotnetVersion)" -ForegroundColor Green
    }
    catch {
        Write-Host "❌ .NET command failed" -ForegroundColor Red
        Write-Host "    Please install .NET 9 from: https://dotnet.microsoft.com/download/dotnet/9.0" -ForegroundColor Red
        Read-Host "Press Enter to continue..."
        exit 1
    }
}
else {
    Write-Host "❌ .NET is not installed or not in PATH" -ForegroundColor Red
    Write-Host "    Please install .NET 9 from: https://dotnet.microsoft.com/download/dotnet/9.0" -ForegroundColor Red
    Read-Host "Press Enter to continue..."
    exit 1
}

# Check if 1Password CLI is installed
Write-Host "[2/2] Checking 1Password CLI installation..." -ForegroundColor White
if (Test-CommandExists "op") {
    try {
        $opVersion = op --version 2>$null
        Write-Host "✅ 1Password CLI is installed (version: $opVersion)" -ForegroundColor Green
    }
    catch {
        Write-Host "❌ 1Password CLI command failed" -ForegroundColor Red
        Write-Host "    Please install from: https://developer.1password.com/docs/cli/get-started/" -ForegroundColor Red
        Read-Host "Press Enter to continue..."
        exit 1
    }
}
else {
    Write-Host "❌ 1Password CLI is not installed or not in PATH" -ForegroundColor Red
    Write-Host "    Please install from: https://developer.1password.com/docs/cli/get-started/" -ForegroundColor Red
    Read-Host "Press Enter to continue..."
    exit 1
}

Write-Host ""
Write-Host "====================================" -ForegroundColor Green
Write-Host "✅ All prerequisites are satisfied!" -ForegroundColor Green
Write-Host "====================================" -ForegroundColor Green
Write-Host ""
Write-Host "Before using the application, please ensure:" -ForegroundColor Yellow
Write-Host "1. You are signed in to 1Password CLI: op signin" -ForegroundColor White
Write-Host "2. You have a secure note named '_CP' in 1Password" -ForegroundColor White
Write-Host "   (The app will create this automatically on first use if it doesn't exist)" -ForegroundColor Gray
Write-Host ""
Write-Host "Building release version..." -ForegroundColor Yellow
try {
    Set-Location ".\ClipboardTo1Pass"
    dotnet build -c Release --verbosity quiet
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ Release build successful" -ForegroundColor Green
    } else {
        Write-Host "❌ Release build failed" -ForegroundColor Red
        Set-Location ".."
        Read-Host "Press Enter to continue..."
        exit 1
    }
    Set-Location ".."
} catch {
    Write-Host "❌ Build error: $_" -ForegroundColor Red
    Set-Location ".."
    Read-Host "Press Enter to continue..."
    exit 1
}

Write-Host ""
Write-Host "The Clipboard to 1Password tool is ready to use." -ForegroundColor White
Write-Host ""
Write-Host "Setup Windows Startup (Optional):" -ForegroundColor Yellow
$startup = Read-Host "Would you like to set up automatic startup on Windows login? (Y/N)"
if ($startup -eq "Y" -or $startup -eq "y") {
    Write-Host ""
    Write-Host "Setting up Windows startup..." -ForegroundColor Yellow
    try {
        $StartupFolder = [Environment]::GetFolderPath("Startup")
        $ExePath = Join-Path $PSScriptRoot "ClipboardTo1Pass\bin\Release\net9.0-windows\ClipboardTo1Pass.exe"
        $ShortcutPath = Join-Path $StartupFolder "ClipboardTo1Pass.lnk"
        
        $WShell = New-Object -ComObject WScript.Shell
        $Shortcut = $WShell.CreateShortcut($ShortcutPath)
        $Shortcut.TargetPath = $ExePath
        $Shortcut.WorkingDirectory = Split-Path $ExePath -Parent
        $Shortcut.Description = "ClipboardTo1Pass - Global hotkey for clipboard to 1Password"
        $Shortcut.Save()
        
        Write-Host "✅ Startup shortcut created successfully" -ForegroundColor Green
        Write-Host "   Location: $ShortcutPath" -ForegroundColor Gray
        Write-Host ""
        Write-Host "The application will now start automatically when you log in." -ForegroundColor Green
        Write-Host "You can disable this in Windows Settings > Apps > Startup" -ForegroundColor Gray
    } catch {
        Write-Host "❌ Failed to create startup shortcut: $_" -ForegroundColor Red
        Write-Host "You can set this up manually later if needed." -ForegroundColor Gray
    }
} else {
    Write-Host ""
    Write-Host "Skipping startup setup. You can run the application manually when needed." -ForegroundColor Gray
}

Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "1. The application is ready to use: .\ClipboardTo1Pass\bin\Release\net9.0-windows\ClipboardTo1Pass.exe" -ForegroundColor White
Write-Host "2. Copy any text to clipboard" -ForegroundColor White
Write-Host "3. Press Ctrl+Alt+F12 to save to 1Password" -ForegroundColor White
Write-Host ""
Write-Host "To remove startup later, delete: $([Environment]::GetFolderPath('Startup'))\ClipboardTo1Pass.lnk" -ForegroundColor Gray
Write-Host ""
Read-Host "Press Enter to continue..."
