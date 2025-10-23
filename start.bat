@echo off
REM ==========================================
REM Game 2048 - Auto Startup Script
REM ==========================================

echo.
echo ========================================
echo    GAME 2048 - DESIGN PATTERNS DEMO
echo ========================================
echo.

REM Check if .NET is installed
where dotnet >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] .NET SDK not found!
    echo Please install .NET SDK from: https://dotnet.microsoft.com/download
    pause
    exit /b 1
)

echo [INFO] Checking .NET version...
dotnet --version

echo.
echo [INFO] Building project...
dotnet build --configuration Release

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [ERROR] Build failed!
    pause
    exit /b 1
)

echo.
echo [SUCCESS] Build completed!
echo.
echo ========================================
echo  Starting Game 2048 Server...
echo ========================================
echo.
echo [INFO] Server will start at: http://localhost:5000
echo [INFO] Press Ctrl+C to stop the server
echo.

REM Wait 2 seconds then open browser
timeout /t 2 /nobreak >nul
start http://localhost:5000

REM Run the application
dotnet run --no-build --configuration Release

pause
