using System.Drawing;
using System.Windows.Forms;
using MarioArcade.Managers;

namespace MarioArcade.Objects
{
    public class Player : GameObject
    {
        public int Score { get; set; }
        private int _lives = 3; // Кол-во жизней у игрока 
        private bool _moveLeft, _moveRight;
        private bool _isJumping = false;
        private float _velocityY = 0f;
        private float _speed = 4f; // Скорость игрока
        private const float Gravity = 0.5f;
        private const float JumpPower = -10.5f; // Сила прыжка
        private const float JumpPowerHit = -8f; // Сила прыжка при ранении
        private bool _facingRight = true;

        // Споры
        internal bool _showSpore = false; // internal для доступа из GameForm
        private float _sporeTimer = 0;
        private const float SporeDuration = 5f;
        internal bool _isRedSpore = true; // internal для доступа из GameForm

        // Физические размеры для корректного прохождения между блоками
        private float _physicalWidth = 28f; 
        private float _physicalHeight = 32f;

        private bool _canJump = true; // Разрешение прыжка

        public Player()
        {        
            // Визуальные размеры игрока
            Width = 32f;
            Height = 32f;
        }

        public float VelocityY => _velocityY;
        public int Lives
        {
            get => _lives;
            set
            {
                // Проверка: когда активна красная спора - игрок неуязвим
                if (_showSpore && _isRedSpore) return;

                if (value < _lives) // Если потерял жизнь
                {
                    _showSpore = true;
                    _sporeTimer = 0;
                    _isRedSpore = true; // Начинаем с красной споры

                    
                    _isJumping = true; // Игрок подпрыгивает
                    _velocityY = JumpPowerHit;
                }
                _lives = value;

                var form = FindGameForm();
                if (form != null)
                {
                    var ui = form.GetUIManager();
                    ui?.UpdateLives(_lives);
                }
            }
        }

        public void PreventJump() // Запрет прыжка
        {
            _canJump = false;
        }

        private GameForm FindGameForm()
        {
            foreach (Form form in System.Windows.Forms.Application.OpenForms)
            {
                if (form is GameForm gameForm)
                    return gameForm;
            }
            return null;
        }

        public override void Update()
        {
            float newX = X;
            if (_moveLeft) newX -= _speed;
            if (_moveRight) newX += _speed;

            // Проверка столкновений по X — используем физические размеры
            bool collisionX = false;
            foreach (var obj in GameForm.CurrentGameObjects)
            {
                if (obj is Block block && block.IsSolid)
                {
                    float tempX = newX;
                    var stub = new GameObjectStub(tempX, Y);
                    stub.Width = _physicalWidth;
                    stub.Height = _physicalHeight;

                    if (CollisionDetector.IsColliding(stub, block))
                    {
                        collisionX = true;
                        break;
                    }
                }
            }

            if (!collisionX)
                X = newX;

            // Запрет уйти за левый край локации
            if (X < 0)
                X = 0;

            // Запрет уйти за правый край локации
            int mapWidth = (int)(GameForm.CurrentLevelManager.Config.MapWidth * GameForm.CurrentLevelManager.Config.BlockSize);
            if (X > mapWidth - Width)
                X = mapWidth - Width;

            // Поворот при движении
            if (_moveRight) _facingRight = true;
            if (_moveLeft) _facingRight = false;

            // Прыжок и падение
            if (_isJumping)
            {
                _velocityY += Gravity;
                Y += _velocityY;

                // Проверка столкновения по Y — используем физические размеры
                bool collisionY = false;
                foreach (var obj in GameForm.CurrentGameObjects)
                {
                    if (obj is Block block && block.IsSolid)
                    {
                        var stub = new GameObjectStub(X, Y);
                        stub.Width = _physicalWidth;
                        stub.Height = _physicalHeight;

                        if (CollisionDetector.IsColliding(stub, block))
                        {
                            if (_velocityY > 0) // Падение
                            {
                                Y = block.Y - _physicalHeight;
                                _velocityY = 0;
                                _isJumping = false;
                            }
                            else if (_velocityY < 0) // Прыжок
                            {
                                Y = block.Y + block.Height;
                                _velocityY = 0;
                            }
                            collisionY = true;
                            break;
                        }
                    }
                }

                if (!collisionY && Y >= GameForm.CurrentLevelManager.GroundY) // Приземление на землю
                {
                    Y = GameForm.CurrentLevelManager.GroundY;
                    _velocityY = 0;
                    _isJumping = false;
                }
            }

            // Проверка: стоит ли на земле или блоке — используем физические размеры
            bool onGround = false;
            foreach (var obj in GameForm.CurrentGameObjects)
            {
                if (obj is Block block && block.IsSolid &&
                    Y + _physicalHeight <= obj.Y && Y + _physicalHeight + 1 >= obj.Y &&
                    X + _physicalWidth > obj.X && X < obj.X + obj.Width)
                {
                    onGround = true;
                    break;
                }
            }

            if (Y >= GameForm.CurrentLevelManager.GroundY) // На уровне земли
                onGround = true;

            if (onGround)
            {
                _isJumping = false;
                _canJump = true; // Разрешаем прыгать снова
            }
            else if (!_isJumping)
            {
                _isJumping = true; // Начинаем падение
                _velocityY = 0;
            }

            // Обновление спор
            if (_showSpore)
            {
                _sporeTimer += 0.016f; // Прибавляем время

                if (_sporeTimer >= 1f && _isRedSpore) // Через 1 секунду
                {
                    _isRedSpore = false; // Меняем красную спору на зелёную спору
                }

                if (_sporeTimer >= SporeDuration)
                {
                    _showSpore = false;
                }
            }
        }

        public override void Draw(Graphics g)
        {
            DrawPlayer(g);
            if (_showSpore)DrawSpore(g); // Отрисовка спор
        }

        private void DrawPlayer(Graphics g)
        {
            string spriteName = _facingRight ? "player_r.png" : "player_l.png";
            var sprite = SpriteRenderer.Load(spriteName);
            var rect = Camera.ApplyCamera(this);
            g.DrawImage(sprite, rect.X, rect.Y, rect.Width, rect.Height);
        }

        private void DrawSpore(Graphics g)
        {
            string spriteName = _isRedSpore ? "spore_r.png" : "spore.png";
            var sprite = SpriteRenderer.Load(spriteName);
            var rect = Camera.ApplyCamera(new GameObjectStub(X, Y - 32)); // Спора рисуется выше игрока
            g.DrawImage(sprite, rect.X, rect.Y, rect.Width, rect.Height);
        }

        // Управление игроком WASD
        public void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A) _moveLeft = true;
            if (e.KeyCode == Keys.D) _moveRight = true;
            if ((e.KeyCode == Keys.W) && !_isJumping && _canJump)
            {
                _isJumping = true;
                _velocityY = JumpPower;
            }
        }

        public void OnKeyUp(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A) _moveLeft = false;
            if (e.KeyCode == Keys.D) _moveRight = false;
        }

        public void ResetKeys() // Для корректности выхода из паузы 
        {
            _moveLeft = false;
            _moveRight = false;
        }
    }

    // Вспомогательный класс для проверки столкновений
    internal class GameObjectStub : GameObject
    {
        public GameObjectStub(float x, float y)
        {
            X = x;
            Y = y;
        }

        public override void Update() { }
        public override void Draw(Graphics g) { }
    }
}