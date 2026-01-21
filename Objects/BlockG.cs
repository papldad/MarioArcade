using System.Drawing;
using MarioArcade.Managers;

namespace MarioArcade.Objects
{
    public class BlockG : Block  // Серый блок с зеленой меткой, который исчезает при подходе игрока снизу
    {
        public BlockG()
        {
            IsSolid = true;
        }

        public override void Update() // Обновление состояния блока
        {
            base.Update();
            CheckPlayerBelow();
        }

        private void CheckPlayerBelow() // Проверяем, находится ли игрок снизу
        {
            foreach (var obj in GameForm.CurrentGameObjects)
            {
                if (obj is Player player && IsPlayerBelow(player))
                {
                    RemoveThisBlock(); // Удаляем блок
                    return;
                }
            }
        }

        private bool IsPlayerBelow(Player player)
        {
            return player.X + player.Width > X &&
                   player.X < X + Width &&
                   player.Y >= Y + Height && // Игрок ниже блока
                   player.Y - 1 <= Y + Height; // Игрок чуть выше нижней границы
        }

        private void RemoveThisBlock() // Удаляем этот блок
        {
            GameForm.CurrentGameObjects.Remove(this);
        }

        public override void Draw(Graphics g)
        {
            var rect = Camera.ApplyCamera(this);
            var sprite = SpriteRenderer.Load("block_g.png");
            g.DrawImage(sprite, rect.X, rect.Y, rect.Width, rect.Height);
        }
    }
}