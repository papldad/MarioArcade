using System.Drawing;
using MarioArcade.Managers;

namespace MarioArcade.Objects
{
    public class Coin : GameObject  // Монета, которая собирается игроком
    {
        public bool IsCollected { get; private set; } = false;  // Собрана ли монета
        private int valueCoinScore = 1; // Сколько добавляется очков за Coin
        private float _targetX, _targetY;  // Координаты анимации монеты
        private float _lerpProgress = 0f;  // Прогресс анимации
        private const float AnimationDuration = 0.5f;  // Длительность анимации

        public void Collect(float targetX, float targetY)  // Начать анимацию
        {
            IsCollected = true;
            _targetX = targetX;
            _targetY = targetY;
        }

        public override void Update()  // Обновление состояния монеты
        {
            CheckPlayerCollision();
            UpdateAnimation();
        }

        private void CheckPlayerCollision()  // Проверить столкновение с игроком
        {
            foreach (var obj in GameForm.CurrentGameObjects)
            {
                if (obj is Player player && !IsCollected && CollisionDetector.IsColliding(this, player))
                {
                    player.Score += valueCoinScore;  // Добавить очки
                    StartCollectionAnimation();  // Начать анимацию
                }
            }
        }

        private void StartCollectionAnimation()  // Начать анимацию сбора
        {
            var gameForm = FindGameForm();
            var ui = gameForm?.GetUIManager();
            var scorePos = ui?.ScoreLabelCenter ?? new Point(20, 20);

            float worldTargetX = scorePos.X + Camera.X;  // Добавляем смещение камеры
            float worldTargetY = scorePos.Y;

            Collect(worldTargetX, worldTargetY);  // Передаём координаты
        }

        private void UpdateAnimation()  // Обновить анимацию
        {
            if (!IsCollected) return;

            _lerpProgress += 0.016f / AnimationDuration;

            if (_lerpProgress >= 1f)
            {
                RemoveThisCoin();  // Удалить монету
            }
        }

        private void RemoveThisCoin()  // Удалить монету
        {
            GameForm.CurrentGameObjects.Remove(this);
        }

        private GameForm FindGameForm()  // Инициализация UI для анимации
        {
            foreach (Form form in System.Windows.Forms.Application.OpenForms)
            {
                if (form is GameForm gameForm)
                    return gameForm;
            }
            return null;
        }

        public override void Draw(Graphics g)
        {
            var drawX = X;
            var drawY = Y;

            if (IsCollected)
            {
                // Интерполируем положение для плавной анимации
                drawX = X + (_targetX - X) * _lerpProgress;
                drawY = Y + (_targetY - Y) * _lerpProgress;
            }

            var rectF = Camera.ApplyCamera(new GameObjectStub(drawX, drawY));
            var rect = new Rectangle(
                (int)rectF.X,
                (int)rectF.Y,
                (int)rectF.Width,
                (int)rectF.Height
            );

            DrawSprite(g, rect);
        }

        private void DrawSprite(Graphics g, Rectangle rect)
        {
            var sprite = SpriteRenderer.Load("coin.png");
            g.DrawImage(sprite, rect.X, rect.Y, rect.Width, rect.Height);
        }
    }
}