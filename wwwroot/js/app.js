// ==========================================
// GAME 2048 - FRONTEND APPLICATION
// Minh họa Design Patterns với C# Backend
// ==========================================

const API_BASE = '/api';

// ==========================================
// STATE MANAGEMENT
// ==========================================
const AppState = {
    currentUser: null,
    currentGridSize: 4,
    currentStrategy: 'standard',
    gameBoard: [],
    score: 0,
    bestScore: 0,
    isGameOver: false,
    hasWon: false,
    winOverlayShown: false,
    touchStartX: 0,
    touchStartY: 0
};

// ==========================================
// SCREEN MANAGEMENT
// ==========================================
function showScreen(screenId) {
    document.querySelectorAll('.screen').forEach(screen => {
        screen.classList.add('hidden');
    });
    document.getElementById(screenId).classList.remove('hidden');
}

// ==========================================
// LOGIN FUNCTIONALITY
// ==========================================
async function handleLogin() {
    const username = document.getElementById('username').value.trim();
    const password = document.getElementById('password').value.trim();
    const errorDiv = document.getElementById('login-error');

    if (!username || !password) {
        errorDiv.textContent = 'Vui lòng nhập đầy đủ thông tin';
        return;
    }

    try {
        const response = await fetch(`${API_BASE}/auth/login`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            credentials: 'include',
            body: JSON.stringify({ username, password })
        });

        const data = await response.json();

        if (data.success) {
            AppState.currentUser = data.username;
            document.getElementById('current-user').textContent = `👤 ${data.username}`;
            
            // Update high scores
            updateHighScores(data.highScores);
            
            errorDiv.textContent = '';
            showScreen('mode-selection-screen');
        } else {
            errorDiv.textContent = 'Sai tên đăng nhập hoặc mật khẩu';
        }
    } catch (error) {
        errorDiv.textContent = 'Lỗi kết nối server';
        console.error('Login error:', error);
    }
}

function updateHighScores(highScores) {
    document.getElementById('high-score-3').textContent = highScores[3] || 0;
    document.getElementById('high-score-4').textContent = highScores[4] || 0;
    document.getElementById('high-score-5').textContent = highScores[5] || 0;
}

async function handleLogout() {
    try {
        await fetch(`${API_BASE}/auth/logout`, { 
            method: 'POST',
            credentials: 'include'
        });
        AppState.currentUser = null;
        showScreen('login-screen');
        document.getElementById('username').value = '';
        document.getElementById('password').value = '';
    } catch (error) {
        console.error('Logout error:', error);
    }
}

// ==========================================
// MODE SELECTION
// ==========================================
function setupModeSelection() {
    const modeCards = document.querySelectorAll('.mode-card');
    
    modeCards.forEach(card => {
        card.addEventListener('click', async () => {
            const gridSize = parseInt(card.dataset.size);
            AppState.currentGridSize = gridSize;
            
            // Set strategy before starting game
            await setStrategy(AppState.currentStrategy);
            
            // Start new game
            await startNewGame(gridSize);
        });
    });
}

// ==========================================
// STRATEGY PATTERN - SELECTION
// ==========================================
function setupStrategySelection() {
    const strategyButtons = document.querySelectorAll('.btn-strategy');
    
    strategyButtons.forEach(btn => {
        btn.addEventListener('click', async () => {
            const strategy = btn.dataset.strategy;
            
            // Update UI
            strategyButtons.forEach(b => b.classList.remove('active'));
            btn.classList.add('active');
            
            // Update state
            AppState.currentStrategy = strategy;
            
            // Update description
            const descriptions = {
                'standard': '✅ Di chuyển và merge theo thuật toán chuẩn, có tính điểm',
                'fast': '⚡ Merge nhiều lần trong 1 lượt, điểm tăng nhanh hơn',
                'test': '🧪 Chỉ di chuyển các ô, KHÔNG merge và KHÔNG tính điểm'
            };
            
            const names = {
                'standard': 'Standard Mode',
                'fast': 'Fast Mode',
                'test': 'Test Mode (No Score)'
            };
            
            document.getElementById('current-strategy').textContent = names[strategy];
            document.getElementById('strategy-description').textContent = descriptions[strategy];
            
            // Set strategy on server
            await setStrategy(strategy);
        });
    });
}

async function setStrategy(strategyType) {
    try {
        await fetch(`${API_BASE}/game/set-strategy`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            credentials: 'include',
            body: JSON.stringify({ strategyType })
        });
        
        // Update game screen info
        const strategyNames = {
            'standard': 'Standard Mode',
            'fast': 'Fast Mode',
            'test': 'Test Mode (No Score)'
        };
        
        const strategyNotes = {
            'standard': '✅ Chế độ chuẩn: Di chuyển và merge bình thường, có tính điểm',
            'fast': '⚡ Chế độ nhanh: Merge nhiều lần trong 1 lượt, điểm tăng nhanh hơn',
            'test': '🧪 Chế độ thử nghiệm: Chỉ di chuyển, KHÔNG merge và KHÔNG tính điểm'
        };
        
        const noteClasses = {
            'standard': '',
            'fast': 'fast-mode',
            'test': 'test-mode'
        };
        
        document.getElementById('game-strategy').textContent = strategyNames[strategyType] || 'Standard';
        
        const noteDiv = document.getElementById('strategy-note');
        if (noteDiv) {
            noteDiv.textContent = strategyNotes[strategyType] || '';
            noteDiv.className = 'strategy-note ' + (noteClasses[strategyType] || '');
        }
    } catch (error) {
        console.error('Set strategy error:', error);
    }
}

// ==========================================
// GAME INITIALIZATION
// ==========================================
async function startNewGame(gridSize) {
    try {
        const response = await fetch(`${API_BASE}/game/new-game`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            credentials: 'include',
            body: JSON.stringify({ gridSize })
        });

        if (!response.ok) {
            console.error('New game failed:', response.status);
            alert('Vui lòng đăng nhập lại!');
            showScreen('login-screen');
            return;
        }

        const data = await response.json();
        
        AppState.gameBoard = data.board;
        AppState.score = data.score;
        AppState.bestScore = data.bestScore;
        AppState.isGameOver = false;
        AppState.hasWon = false;
        AppState.winOverlayShown = false;
        
        updateGameUI(data);
        initializeGrid(gridSize);
        renderBoard(data.board, gridSize);
        
        showScreen('game-screen');
        
        // Update mode display
        document.getElementById('current-mode').textContent = `${gridSize}x${gridSize}`;
    } catch (error) {
        console.error('New game error:', error);
        alert('Lỗi kết nối server. Vui lòng thử lại!');
    }
}

function initializeGrid(gridSize) {
    const container = document.getElementById('grid-container');
    container.innerHTML = '';
    
    const grid = document.createElement('div');
    grid.className = `grid grid-size-${gridSize}`;
    grid.id = 'game-grid';
    
    // Create cells
    for (let i = 0; i < gridSize * gridSize; i++) {
        const cell = document.createElement('div');
        cell.className = 'cell tile-0';
        cell.dataset.index = i;
        grid.appendChild(cell);
    }
    
    container.appendChild(grid);
}

// ==========================================
// BOARD RENDERING
// Minh họa DECORATOR PATTERN cho cells
// ==========================================
function renderBoard(board, gridSize) {
    const cells = document.querySelectorAll('#game-grid .cell');
    
    board.forEach((row, i) => {
        row.forEach((value, j) => {
            const index = i * gridSize + j;
            const cell = cells[index];
            
            if (cell) {
                const oldValue = parseInt(cell.dataset.value) || 0;
                cell.dataset.value = value;
                
                // Base cell rendering
                cell.textContent = value === 0 ? '' : value;
                cell.className = `cell tile-${value}`;
                
                // DECORATOR PATTERN: Apply decorators based on value
                if (value > 0) {
                    // New tile decorator
                    if (oldValue === 0 && value > 0) {
                        cell.classList.add('new-tile');
                        setTimeout(() => cell.classList.remove('new-tile'), 300);
                    }
                    
                    // Merged tile decorator
                    if (oldValue > 0 && value !== oldValue) {
                        cell.classList.add('merged-tile');
                        setTimeout(() => cell.classList.remove('merged-tile'), 300);
                    }
                    
                    // Special effect decorator for high values
                    if (value >= 512) {
                        cell.classList.add('special-glow');
                    }
                    
                    // Trophy decorator for very high values
                    if (value >= 1024) {
                        cell.textContent = '🏆 ' + value;
                    }
                }
            }
        });
    });
}

function updateGameUI(data) {
    document.getElementById('current-score').textContent = data.score;
    document.getElementById('best-score').textContent = data.bestScore;
    
    // Check game state
    if (data.won && !AppState.winOverlayShown) {
        showWinOverlay(data.score);
        AppState.winOverlayShown = true;
    } else if (data.gameOver) {
        showGameOverOverlay(data.score);
    }
}

// ==========================================
// MOVEMENT HANDLING (Strategy Pattern on server)
// ==========================================
async function handleMove(direction) {
    if (AppState.isGameOver) return;
    
    try {
        const response = await fetch(`${API_BASE}/game/move`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            credentials: 'include',
            body: JSON.stringify({ direction })
        });

        const data = await response.json();
        
        AppState.gameBoard = data.board;
        AppState.score = data.score;
        AppState.bestScore = data.bestScore;
        
        renderBoard(data.board, AppState.currentGridSize);
        updateGameUI(data);
        
        // Save score to server
        if (data.score > 0) {
            saveScore(AppState.currentGridSize, data.score);
        }
    } catch (error) {
        console.error('Move error:', error);
    }
}

async function saveScore(gridSize, score) {
    try {
        const response = await fetch(`${API_BASE}/auth/save-score`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            credentials: 'include',
            body: JSON.stringify({ gridSize, score })
        });
        
        const data = await response.json();
        if (data.success) {
            updateHighScores(data.highScores);
        }
    } catch (error) {
        console.error('Save score error:', error);
    }
}

// ==========================================
// KEYBOARD CONTROLS
// ==========================================
function setupKeyboardControls() {
    document.addEventListener('keydown', (e) => {
        // Only handle if game screen is visible
        if (document.getElementById('game-screen').classList.contains('hidden')) {
            return;
        }
        
        const keyMap = {
            'ArrowUp': 'up',
            'ArrowDown': 'down',
            'ArrowLeft': 'left',
            'ArrowRight': 'right'
        };
        
        if (keyMap[e.key]) {
            e.preventDefault();
            handleMove(keyMap[e.key]);
        }
    });
}

// ==========================================
// TOUCH CONTROLS
// ==========================================
function setupTouchControls() {
    const gridContainer = document.getElementById('grid-container');
    
    gridContainer.addEventListener('touchstart', (e) => {
        AppState.touchStartX = e.touches[0].clientX;
        AppState.touchStartY = e.touches[0].clientY;
    }, { passive: true });
    
    gridContainer.addEventListener('touchend', (e) => {
        if (!AppState.touchStartX || !AppState.touchStartY) return;
        
        const touchEndX = e.changedTouches[0].clientX;
        const touchEndY = e.changedTouches[0].clientY;
        
        const diffX = touchEndX - AppState.touchStartX;
        const diffY = touchEndY - AppState.touchStartY;
        
        // Minimum swipe distance
        const minSwipe = 30;
        
        if (Math.abs(diffX) > Math.abs(diffY)) {
            // Horizontal swipe
            if (Math.abs(diffX) > minSwipe) {
                handleMove(diffX > 0 ? 'right' : 'left');
            }
        } else {
            // Vertical swipe
            if (Math.abs(diffY) > minSwipe) {
                handleMove(diffY > 0 ? 'down' : 'up');
            }
        }
        
        AppState.touchStartX = 0;
        AppState.touchStartY = 0;
    }, { passive: true });
}

// ==========================================
// OVERLAYS
// ==========================================
function showGameOverOverlay(score) {
    AppState.isGameOver = true;
    document.getElementById('final-score').textContent = score;
    document.getElementById('game-over-overlay').classList.remove('hidden');
}

function showWinOverlay(score) {
    document.getElementById('win-score').textContent = score;
    document.getElementById('win-overlay').classList.remove('hidden');
}

function hideOverlays() {
    document.getElementById('game-over-overlay').classList.add('hidden');
    document.getElementById('win-overlay').classList.add('hidden');
}

// ==========================================
// BUTTON HANDLERS
// ==========================================
function setupGameButtons() {
    // New Game button
    document.getElementById('new-game-btn').addEventListener('click', () => {
        hideOverlays();
        startNewGame(AppState.currentGridSize);
    });
    
    // Change Mode button
    document.getElementById('change-mode-btn').addEventListener('click', () => {
        hideOverlays();
        showScreen('mode-selection-screen');
    });
    
    // Try Again button (Game Over)
    document.getElementById('try-again-btn').addEventListener('click', () => {
        hideOverlays();
        startNewGame(AppState.currentGridSize);
    });
    
    // Back to Modes button (Game Over)
    document.getElementById('back-to-modes-btn').addEventListener('click', () => {
        hideOverlays();
        showScreen('mode-selection-screen');
    });
    
    // Continue button (Win)
    document.getElementById('continue-btn').addEventListener('click', () => {
        hideOverlays();
        AppState.winOverlayShown = true; // Don't show again
    });
    
    // New Game button (Win)
    document.getElementById('new-game-win-btn').addEventListener('click', () => {
        hideOverlays();
        startNewGame(AppState.currentGridSize);
    });
    
    // Logout button
    document.getElementById('logout-btn').addEventListener('click', handleLogout);
    
    // Login button
    document.getElementById('login-btn').addEventListener('click', handleLogin);
    
    // Enter key on login
    document.getElementById('password').addEventListener('keypress', (e) => {
        if (e.key === 'Enter') {
            handleLogin();
        }
    });
}

// ==========================================
// INITIALIZATION
// ==========================================
function init() {
    console.log('🎮 Game 2048 - Design Patterns Demo');
    console.log('📐 Singleton Pattern: GameManagerSingleton quản lý state');
    console.log('🎨 Decorator Pattern: Cell decorators cho effects');
    console.log('⚡ Strategy Pattern: Các thuật toán di chuyển khác nhau');
    
    setupModeSelection();
    setupStrategySelection();
    setupGameButtons();
    setupKeyboardControls();
    setupTouchControls();
    
    // Show login screen initially
    showScreen('login-screen');
}

// Start application when DOM is ready
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', init);
} else {
    init();
}
