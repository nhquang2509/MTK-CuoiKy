# Test API Script cho Game 2048 Database

# Cấu hình
$baseUrl = "http://localhost:5000"

Write-Host "🎮 Testing Game 2048 API với Database" -ForegroundColor Cyan
Write-Host "========================================`n" -ForegroundColor Cyan

# Test 1: Register new user
Write-Host "✅ Test 1: Register new user" -ForegroundColor Green
$registerBody = @{
    Username = "testplayer"
    Password = "test123"
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/auth/register" `
        -Method POST `
        -Body $registerBody `
        -ContentType "application/json"
    
    Write-Host "   Response: $($response | ConvertTo-Json)" -ForegroundColor Yellow
} catch {
    Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""

# Test 2: Login
Write-Host "✅ Test 2: Login with player1" -ForegroundColor Green
$loginBody = @{
    Username = "player1"
    Password = "pass123"
} | ConvertTo-Json

try {
    $session = New-Object Microsoft.PowerShell.Commands.WebRequestSession
    
    $response = Invoke-RestMethod -Uri "$baseUrl/api/auth/login" `
        -Method POST `
        -Body $loginBody `
        -ContentType "application/json" `
        -WebSession $session
    
    Write-Host "   Username: $($response.username)" -ForegroundColor Yellow
    Write-Host "   High Scores:" -ForegroundColor Yellow
    $response.highScores.PSObject.Properties | ForEach-Object {
        Write-Host "     - Grid $($_.Name)x$($_.Name): $($_.Value)" -ForegroundColor White
    }
} catch {
    Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""

# Test 3: Save Score
Write-Host "✅ Test 3: Save high score" -ForegroundColor Green
$scoreBody = @{
    GridSize = 4
    Score = 2048
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/auth/save-score" `
        -Method POST `
        -Body $scoreBody `
        -ContentType "application/json" `
        -WebSession $session
    
    Write-Host "   Success: $($response.success)" -ForegroundColor Yellow
    Write-Host "   Updated High Scores:" -ForegroundColor Yellow
    $response.highScores.PSObject.Properties | ForEach-Object {
        Write-Host "     - Grid $($_.Name)x$($_.Name): $($_.Value)" -ForegroundColor White
    }
} catch {
    Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""

# Test 4: Get Current User
Write-Host "✅ Test 4: Get current user" -ForegroundColor Green
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/auth/current" `
        -Method GET `
        -WebSession $session
    
    Write-Host "   Username: $($response.username)" -ForegroundColor Yellow
    Write-Host "   High Scores:" -ForegroundColor Yellow
    $response.highScores.PSObject.Properties | ForEach-Object {
        Write-Host "     - Grid $($_.Name)x$($_.Name): $($_.Value)" -ForegroundColor White
    }
} catch {
    Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""

# Test 5: Leaderboard
Write-Host "✅ Test 5: Get leaderboard (Grid 4x4, Top 10)" -ForegroundColor Green
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/auth/leaderboard?gridSize=4&top=10" `
        -Method GET
    
    Write-Host "   Leaderboard:" -ForegroundColor Yellow
    $rank = 1
    $response.leaderboard | ForEach-Object {
        Write-Host "     $rank. $($_.username): $($_.score) points" -ForegroundColor White
        $rank++
    }
} catch {
    Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""

# Test 6: Logout
Write-Host "✅ Test 6: Logout" -ForegroundColor Green
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/auth/logout" `
        -Method POST `
        -WebSession $session
    
    Write-Host "   Success: $($response.success)" -ForegroundColor Yellow
} catch {
    Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""

# Check users.json file
Write-Host "📁 Checking Data/users.json file" -ForegroundColor Cyan
if (Test-Path "Data/users.json") {
    Write-Host "   File exists! Content:" -ForegroundColor Green
    $content = Get-Content "Data/users.json" -Raw | ConvertFrom-Json
    Write-Host "   Total users: $($content.Count)" -ForegroundColor Yellow
    $content | ForEach-Object {
        Write-Host "   - Username: $($_.Username)" -ForegroundColor White
        Write-Host "     High Scores:" -ForegroundColor White
        $_.HighScores.PSObject.Properties | ForEach-Object {
            Write-Host "       * Grid $($_.Name)x$($_.Name): $($_.Value)" -ForegroundColor Gray
        }
    }
} else {
    Write-Host "   File not found!" -ForegroundColor Red
}

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "✅ All tests completed!" -ForegroundColor Green
