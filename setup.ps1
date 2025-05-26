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
Write-Host "[1/4] Checking .NET 9 installation..." -ForegroundColor White
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
Write-Host "[2/4] Checking 1Password CLI installation..." -ForegroundColor White
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

# Check if signed in to 1Password CLI
Write-Host "[3/4] Checking 1Password CLI authentication..." -ForegroundColor White
try {
    $whoami = op whoami 2>$null
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ 1Password CLI is authenticated (signed in as: $whoami)" -ForegroundColor Green
    }
    else {
        Write-Host "❌ Not signed in to 1Password CLI" -ForegroundColor Red
        Write-Host "    Please run: op signin" -ForegroundColor Red
        Read-Host "Press Enter to continue..."
        exit 1
    }
}
catch {
    Write-Host "❌ 1Password CLI authentication check failed" -ForegroundColor Red
    Write-Host "    Please run: op signin" -ForegroundColor Red
    Read-Host "Press Enter to continue..."
    exit 1
}

# Check if secure note exists
Write-Host "[4/4] Checking for secure note '_CP'..." -ForegroundColor White
try {
    $item = op item get "_CP" --format=json 2>$null
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ Secure note '_CP' found" -ForegroundColor Green
    }
    else {
        Write-Host "❌ Secure note '_CP' not found" -ForegroundColor Red
        Write-Host ""
        $create = Read-Host "Would you like to create it now? (Y/N)"
        if ($create -eq "Y" -or $create -eq "y") {
            Write-Host "Creating secure note '_CP'..." -ForegroundColor Yellow
            try {
                op item create --category="Secure Note" --title="_CP" notesPlain="Clipboard content will be saved here automatically." 2>$null
                if ($LASTEXITCODE -eq 0) {
                    Write-Host "✅ Secure note '_CP' created successfully" -ForegroundColor Green
                }
                else {
                    Write-Host "❌ Failed to create secure note" -ForegroundColor Red
                    Read-Host "Press Enter to continue..."
                    exit 1
                }
            }
            catch {
                Write-Host "❌ Exception creating secure note" -ForegroundColor Red
                Read-Host "Press Enter to continue..."
                exit 1
            }
        }
        else {
            Write-Host "Please create a secure note named '_CP' manually in 1Password" -ForegroundColor Red
            Read-Host "Press Enter to continue..."
            exit 1
        }
    }
}
catch {
    Write-Host "❌ Error checking for secure note" -ForegroundColor Red
    Read-Host "Press Enter to continue..."
    exit 1
}

Write-Host ""
Write-Host "====================================" -ForegroundColor Green
Write-Host "✅ All prerequisites are satisfied!" -ForegroundColor Green
Write-Host "====================================" -ForegroundColor Green
Write-Host ""
Write-Host "The Clipboard to 1Password tool is ready to use." -ForegroundColor White
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "1. Run the application: .\ClipboardTo1Pass\bin\Release\net9.0-windows\ClipboardTo1Pass.exe" -ForegroundColor White
Write-Host "2. Copy any text to clipboard" -ForegroundColor White
Write-Host "3. Press Ctrl+Shift+V to save to 1Password" -ForegroundColor White
Write-Host ""
Read-Host "Press Enter to continue..."
