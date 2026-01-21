using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MarioArcade.Managers;
using MarioArcade.Data;
using MarioArcade.Objects;
using MarioArcade.UI;
using Timer = System.Windows.Forms.Timer;

namespace MarioArcade
{
    public partial class GameForm : Form
    {
        private Timer _gameTimer;
        private List<GameObject> _gameObjects;
        private Player _player;
        private Image _background;
        private int _mapWidth;
        private UIManager _uiManager;
        private RenderManager _renderManager;
        private InputManager _inputManager;
        private GameLogicManager _gameLogicManager;

        public static List<GameObject> CurrentGameObjects;
        public static LevelManager CurrentLevelManager { get; private set; }

        public GameForm()
        {
            InitializeComponent();
            SetupGame();
        }

        public UIManager GetUIManager() => _uiManager;

        private void SetupGame()
        {
            this.ClientSize = new Size(800, 600);
            this.Text = "Mario";
            this.KeyPreview = true;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);

            // UI
            _uiManager = new UIManager(
                onScoreChanged: (score) => { },
                onRestartRequested: () => RestartLevel(),
                onPauseToggled: (paused) => TogglePause(paused)
            );
            _uiManager.AddControlsTo(this);

            LoadLevel("Levels\\level1.json");

            _gameTimer = new Timer();
            _gameTimer.Interval = 16;
            _gameTimer.Tick += GameTimer_Tick;
            _gameTimer.Start();
        }

        private void LoadLevel(string levelPath)
        {
            _background = SpriteRenderer.Load("background.png");

            var levelManager = new LevelManager(levelPath);
            CurrentLevelManager = levelManager;
            _gameObjects = levelManager.CreateObjects();
            _mapWidth = (int)(levelManager.Config.MapWidth * levelManager.Config.BlockSize);

            _player = (Player)_gameObjects.First(o => o is Player);

            CurrentGameObjects = _gameObjects;
            _renderManager = new RenderManager(levelManager, _background, _mapWidth);
            _inputManager = new InputManager(_player);
            _gameLogicManager = new GameLogicManager(
                _player,
                _gameObjects,
                score => _uiManager.UpdateScore(score)
            );

            Camera.Initialize(_player, _mapWidth); // Инициализация камеры
            
            _uiManager.UpdateLives(_player.Lives); // Обновляем UI
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (_uiManager.IsPaused) return;

            // Копируем список перед итерацией для обновления
            var gameObjectsCopy = new List<GameObject>(_gameObjects);
            foreach (var obj in gameObjectsCopy)
                obj.Update();

            // Вызываем логику игры (для обновления счёта)
            _gameLogicManager.ProcessGameLogic();

            // Обновляем камеру
            Camera.X = 0;
            Camera.Update();

            // Проверка количества жизней
            if (_player.Lives <= 0)
            {
                TogglePause(true);
                _gameTimer.Enabled = false;
                _uiManager.ShowGameOver(_player.Score);

                RestartLevel();
                return;
            }

            // Проверка финиша
            GameObject finishToDestroy = null;
            foreach (var obj in _gameObjects)
            {
                if (obj is Flag flag && flag.Type == FlagType.Finish && CollisionDetector.IsColliding(_player, flag))
                {
                    finishToDestroy = obj;

                    _gameObjects.Remove(finishToDestroy);

                    TogglePause(true);
                    _gameTimer.Enabled = false;

                    _uiManager.ShowWin(_player.Score);
                    RestartLevel();
                    return;
                }
            }

            this.Invalidate();
        }

        private void RestartLevel()
        {
            _gameTimer.Stop();
            this.Controls.Clear();
            SetupGame();
        }

        private void TogglePause(bool paused)
        {
            _gameTimer.Enabled = !paused;
            _inputManager.SetPausedState(paused);
            _gameLogicManager.SetPausedState(paused);

            if (paused)
            {
                _inputManager.ResetPlayerKeys();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            _renderManager.Draw(e.Graphics, Camera.X);

            foreach (var obj in _gameObjects)
            {
                if (obj is Player player && player._showSpore)  // Для корректоного отображения спор
                {
                    player.Draw(e.Graphics);
                }
            }

            _uiManager.DrawLives(e.Graphics);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            _inputManager.HandleKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            _inputManager.HandleKeyUp(e);
        }
    }
}