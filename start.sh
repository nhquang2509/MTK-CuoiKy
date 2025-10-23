#!/bin/bash
# ==========================================
# Game 2048 - Auto Startup Script (Linux/Mac)
# ==========================================

echo ""
echo "========================================"
echo "   GAME 2048 - DESIGN PATTERNS DEMO"
echo "========================================"
echo ""

# Check if .NET is installed
if ! command -v dotnet &> /dev/null; then
    echo "[ERROR] .NET SDK not found!"
    echo "Please install .NET SDK from: https://dotnet.microsoft.com/download"
    read -p "Press Enter to exit"
    exit 1
fi

echo "[INFO] .NET SDK version: $(dotnet --version)"

echo ""
echo "[INFO] Building project..."
dotnet build --configuration Release

if [ $? -ne 0 ]; then
    echo ""
    echo "[ERROR] Build failed!"
    read -p "Press Enter to exit"
    exit 1
fi

echo ""
echo "[SUCCESS] Build completed!"
echo ""
echo "========================================"
echo " Starting Game 2048 Server..."
echo "========================================"
echo ""
echo "[INFO] Server will start at: http://localhost:5000"
echo "[INFO] Press Ctrl+C to stop the server"
echo ""

# Wait 3 seconds then open browser
sleep 3

# Open browser based on OS
if [[ "$OSTYPE" == "linux-gnu"* ]]; then
    xdg-open http://localhost:5000 &
elif [[ "$OSTYPE" == "darwin"* ]]; then
    open http://localhost:5000 &
fi

# Run the application
dotnet run --no-build --configuration Release
