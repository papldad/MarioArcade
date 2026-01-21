using System;
using System.Drawing;
using System.Windows.Forms;
using MarioArcade.Managers;

namespace MarioArcade.UI
{
    public class UIManager
    {
        public Label ScoreLabel { get; private set; }
        public Button RestartButton { get; private set; }
        public Button PauseButton { get; private set; }

        private bool _isPaused = false;
        public bool IsPaused => _isPaused;

        // Параметры и размеры отображения UI
        public int locationWidthUI = 10;
        public int locationHeightUI = 10;
        public int sizeBtnWidth = 100;
        public int sizeBtnHeight = 40;
        public int sizeMarginUI = 10;
        public Point ScoreLabelCenter => new Point(
            ScoreLabel.Location.X + ScoreLabel.Width / 2,
            ScoreLabel.Location.Y + ScoreLabel.Height / 2
        );

        private Action<int> _onScoreChanged;
        private Action _onRestartRequested;
        private Action<bool> _onPauseToggled;
        private Image _heartImage;
        private int _livesCount;

        public UIManager(Action<int> onScoreChanged, Action onRestartRequested, Action<bool> onPauseToggled)
        {
            _onScoreChanged = onScoreChanged;
            _onRestartRequested = onRestartRequested;
            _onPauseToggled = onPauseToggled;

            InitializeUI();
        }

        private void InitializeUI()
        {
            LoadHeartImage(); // Загрузка изображения сердца
            CreateScoreLabel(); // Создание надписи счёта
            CreateRestartButton(); // Создание кнопки перезапуска
            CreatePauseButton(); // Создание кнопки паузы
        }

        private void LoadHeartImage()
        {
            _heartImage = SpriteRenderer.Load("health.png");
        }

        private void CreateScoreLabel()
        {
            ScoreLabel = new Label
            {
                Text = "Score: 0",
                Location = new Point(locationWidthUI, locationHeightUI + sizeBtnHeight + sizeMarginUI),
                Size = new Size(sizeBtnWidth * 2 + sizeMarginUI, sizeBtnHeight),
                ForeColor = Color.Black,
                BackColor = Color.Gold,
                Font = new Font("Arial", 14, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };
        }

        private void CreateRestartButton()
        {
            RestartButton = new Button
            {
                Text = "Restart",
                Location = new Point(locationWidthUI, locationHeightUI),
                Size = new Size(sizeBtnWidth, sizeBtnHeight),
                BackColor = Color.FromArgb(75, 150, 255),
                ForeColor = Color.White,
                Font = new Font("Arial", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            RestartButton.FlatAppearance.BorderSize = 0;
            RestartButton.Click += OnRestartClicked;
        }

        private void CreatePauseButton()
        {
            PauseButton = new Button
            {
                Text = "Pause",
                Location = new Point(locationWidthUI + sizeBtnWidth + sizeMarginUI, locationHeightUI),
                Size = new Size(sizeBtnWidth, sizeBtnHeight),
                BackColor = Color.FromArgb(255, 100, 100),
                ForeColor = Color.White,
                Font = new Font("Arial", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            PauseButton.FlatAppearance.BorderSize = 0;
            PauseButton.Click += OnPauseClicked;
        }

        private void OnRestartClicked(object sender, EventArgs e)
        {
            _onRestartRequested?.Invoke();
        }

        private void OnPauseClicked(object sender, EventArgs e)
        {
            _isPaused = !_isPaused;
            PauseButton.Text = _isPaused ? "Play" : "Pause";
            _onPauseToggled?.Invoke(_isPaused);
        }

        public void ShowWin(int score)
        {
            MessageBox.Show($"WINNER!\nСчёт: {score}", "The End!", MessageBoxButtons.OK);
        }

        public void ShowGameOver(int score)
        {
            MessageBox.Show($"GAME OVER!\nСчёт: {score}", "The End!", MessageBoxButtons.OK);
        }

        public void UpdateScore(int score)
        {
            _onScoreChanged?.Invoke(score);
            ScoreLabel.Text = $"Score: {score}";
        }

        public void UpdateLives(int lives)
        {
            _livesCount = lives;
        }

        public void DrawLives(Graphics g)
        {
            if (_heartImage == null) return;

            for (int i = 0; i < _livesCount; i++)
            {
                g.DrawImage(_heartImage, 750 - i * 35, 10, 30, 30);
            }
        }

        public void AddControlsTo(Form form)
        {
            form.Controls.Add(ScoreLabel);
            form.Controls.Add(RestartButton);
            form.Controls.Add(PauseButton);
        }
    }
}