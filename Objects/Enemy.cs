using System;
using System.Drawing;
using MarioArcade.Managers;

namespace MarioArcade.Objects
{
    public class Enemy : GameObject  // Враг, который движется и реагирует на игрока
    {
        public string InitialDirection = "stop";  // Начальное направление: "left", "right", "stop"
        private bool _hasSetDirection = false;  // Установлено ли начальное направление
        private float _speed = 2f;  // Скорость движения
        private float _velocityY = 0f;  // Скорость по Y
        private const float Gravity = 0.5f;  // Гравитация

        private bool _isDead = false;  // Умер ли враг
        private float _deathTimer = 0;  // Таймер смерти
        private const float DeathDuration = 5f;  // Длительность смерти

        private int valueEnemyScore = 2; // Сколько добавляется очков за Enemy

        // Отброс
        private bool _isKnockedBack = false;  // Отброшен ли враг при столкновении с игроком
        private float _knockbackSpeed = 5f;  // Скорость отброса
        private float _knockbackTimer = 0f;  // Таймер отброса
        private const float KnockbackDuration = 0.5f;  // Длительность отброса

        // Движение
        private bool _isStopped = false;  // Остановлен ли враг
        private bool _movingRight = false;  // Движется ли вправо

        // Физические размеры для корректного "прохода" между блоками
        private float _physicalWidth = 28f;
        private float _physicalHeight = 30f;

        public override void Update()  // Обновление состояния врага
        {
            int mapWidth = (int)(GameForm.CurrentLevelManager.Config.MapWidth * GameForm.CurrentLevelManager.Config.BlockSize);

            if (_isDead)
            {
                UpdateDeathTimer();  // Обновить таймер смерти
                return;
            }

            HandleKnockback(mapWidth);  // Обработать отброс
            SetInitialDirection();  // Установить начальное направление
            MoveIfNotStopped(mapWidth);  // Обработать движение врага
            ApplyGravity();  // Применить гравитацию
            HandlePlayerCollision();  // Обработать столкновение с игроком
        }

        private void UpdateDeathTimer()  // Обновить таймер смерти
        {
            _deathTimer += 0.016f;
            if (_deathTimer >= DeathDuration)
            {
                RemoveThisEnemy();  // Удалить врага
            }
        }

        private void HandleKnockback(int mapWidth)  // Обработать отброс
        {
            if (!_isKnockedBack) return;

            _knockbackTimer += 0.016f;

            if (_knockbackTimer >= KnockbackDuration)
            {
                _isKnockedBack = false;
                _knockbackTimer = 0f;
                return;
            }

            float newX = X - _knockbackSpeed * (_movingRight ? 1 : -1);

            if (!IsCollisionWithBlocksOrEdges(newX, mapWidth))
            {
                X = newX;
            }
            else
            {
                _isKnockedBack = false;
                _knockbackTimer = 0f;
            }
        }

        private void SetInitialDirection()  // Установить начальное направление
        {
            if (_isKnockedBack || _hasSetDirection) return;

            _isStopped = InitialDirection == "stop";
            _movingRight = InitialDirection == "right";
            _hasSetDirection = true;
        }

        private void MoveIfNotStopped(int mapWidth)  // Двигаться, если не задано движение
        {
            if (_isStopped || _isKnockedBack) return;

            MoveHorizontally(mapWidth);
        }

        private void MoveHorizontally(int mapWidth)  // Двигаться по горизонтали
        {
            float newX = X;
            if (_movingRight) newX += _speed;
            else newX -= _speed;

            if (!IsCollisionWithBlocksOrEdges(newX, mapWidth))
            {
                X = newX;
            }
            else
            {
                _movingRight = !_movingRight;
            }
        }

        private bool IsCollisionWithBlocksOrEdges(float newX, int mapWidth)  // Проверить столкновение с блоками или краями локации
        {
            foreach (var obj in GameForm.CurrentGameObjects)
            {
                if (obj is Block block && block.IsSolid)
                {
                    var stub = new GameObjectStub(newX, Y);
                    stub.Width = _physicalWidth;
                    stub.Height = _physicalHeight;

                    if (CollisionDetector.IsColliding(stub, block))
                    {
                        return true;
                    }
                }
            }

            return newX <= 0 || newX >= mapWidth - _physicalWidth;
        }

        private void ApplyGravity()  // Применить гравитацию
        {
            _velocityY += Gravity;
            Y += _velocityY;

            HandleVerticalCollision();  // Обработать столкновение по Y
            HandleGroundCollision();  // Обработать столкновение с землей
        }

        private void HandleVerticalCollision()  // Обработать столкновение по Y
        {
            foreach (var obj in GameForm.CurrentGameObjects)
            {
                if (obj is Block block && block.IsSolid)
                {
                    var stub = new GameObjectStub(X, Y);
                    stub.Width = _physicalWidth;
                    stub.Height = _physicalHeight;

                    if (CollisionDetector.IsColliding(stub, block))
                    {
                        if (_velocityY > 0)  // Падение
                        {
                            Y = block.Y - _physicalHeight;
                            _velocityY = 0;
                        }
                        return;
                    }
                }
            }
        }

        private void HandleGroundCollision()  // Обработать столкновение с землей
        {
            bool onGround = false;
            foreach (var obj in GameForm.CurrentGameObjects)
            {
                if (obj is Ground ground &&
                    Y + _physicalHeight <= obj.Y && Y + _physicalHeight + 2 >= obj.Y &&
                    X + _physicalWidth > obj.X && X < obj.X + obj.Width)
                {
                    onGround = true;
                    break;
                }
            }

            if (Y >= GameForm.CurrentLevelManager.GroundY)
                onGround = true;

            if (onGround)
            {
                Y = GameForm.CurrentLevelManager.GroundY;
                _velocityY = 0;
            }
        }

        private void HandlePlayerCollision()  // Обработать столкновение с игроком
        {
            foreach (var obj in GameForm.CurrentGameObjects)
            {
                if (obj is Player player && IsPlayerColliding(player))
                {
                    HandlePlayerHit(player);  // Обработать удар игрока
                    return;
                }
            }
        }

        private bool IsPlayerColliding(Player player)  // Проверить столкновение с игроком
        {
            var enemyStub = new GameObjectStub(X, Y);
            enemyStub.Width = _physicalWidth;
            enemyStub.Height = _physicalHeight;

            return CollisionDetector.IsColliding(enemyStub, player);
        }

        private void HandlePlayerHit(Player player)  // Обработать удар игрока
        {
            // Проверяем, прыгнул ли игрок сверху
            if (player.VelocityY > 0 &&  // Игрок падает вниз
                player.Y < Y - 2)  // Координата игрока выше врага; 2 - разница между визуальным размером врага и физическим
            {
                KillEnemy(player);  // Убить врага
            }
            else  // Игрок не сверху — наносим урон игроку
            {
                DamagePlayer(player);  // Нанести урон игроку
            }
        }

        private void KillEnemy(Player player)  // Убить врага
        {
            _isDead = true;
            player.Score += valueEnemyScore;
        }

        private void DamagePlayer(Player player)  // Нанести урон игроку
        {
            player.Lives--;

            if (_isStopped)
            {
                _isStopped = false;
            }

            _movingRight = player.X > X;  // Отбрасываем в противоположную сторону от игрока
            _isKnockedBack = true;
            _knockbackTimer = 0f;
        }

        private void RemoveThisEnemy()  // Удалить этого врага
        {
            GameForm.CurrentGameObjects.Remove(this);
        }

        public override void Draw(Graphics g)
        {
            string spriteName = _isDead ? "enemy_d.png" : "enemy.png";
            var sprite = SpriteRenderer.Load(spriteName);
            var rect = Camera.ApplyCamera(this);
            g.DrawImage(sprite, rect.X, rect.Y, rect.Width, rect.Height);
        }
    }
}