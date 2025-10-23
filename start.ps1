# ==========================================
# Game 2048 - Auto Startup Script (PowerShell)
# ==========================================

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   GAME 2048 - DESIGN PATTERNS DEMO" -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if .NET is installed
try {
    $dotnetVersion = dotnet --version
    Write-Host "[INFO] .NET SDK version: $dotnetVersion" -ForegroundColor Green
} catch {
    Write-Host "[ERROR] .NET SDK not found!" -ForegroundColor Red
    Write-Host "Please install .NET SDK from: https://dotnet.microsoft.com/download" -ForegroundColor Yellow
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Host ""
Write-Host "[INFO] Building project..." -ForegroundColor Cyan
dotnet build --configuration Release

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "[ERROR] Build failed!" -ForegroundColor Red
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Host ""
Write-Host "[SUCCESS] Build completed!" -ForegroundColor Green
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host " Starting Game 2048 Server..." -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "[INFO] Server will start at: http://localhost:5000" -ForegroundColor Green
Write-Host "[INFO] Press Ctrl+C to stop the server" -ForegroundColor Yellow
Write-Host ""

# Wait 3 seconds then open browser in background
Start-Sleep -Seconds 3
Start-Process "http://localhost:5000"

# Run the application
dotnet run --no-build --configuration Release
