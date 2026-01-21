using System.Drawing;
using MarioArcade.Managers;

namespace MarioArcade.Objects
{
    public class BlockR : Block  // Серый блок с красной меткой, который исчезает при подходе игрока сверху
    {
        public BlockR()
        {
            IsSolid = true;
        }

        public override void Update()  // Обновление состояния блока
        {
            base.Update();
            CheckPlayerOnTop();
        }

        private void CheckPlayerOnTop() // Проверяем, стоит ли игрок на блоке
        {
            foreach (var obj in GameForm.CurrentGameObjects)
            {
                if (obj is Player player && IsPlayerOnTop(player))
                {
                    RemoveThisBlock(); // Удаляем блок
                    player.PreventJump(); // Ограничения прыжка у игрока при удалении блока
                    return;
                }
            }
        }

        private bool IsPlayerOnTop(Player player)
        {
            return player.X + player.Width > X &&
                   player.X < X + Width &&
                   player.Y + player.Height <= Y &&
                   player.Y + player.Height + 1 >= Y;
        }

        private void RemoveThisBlock()
        {
            GameForm.CurrentGameObjects.Remove(this);
        }

        public override void Draw(Graphics g)
        {
            var sprite = SpriteRenderer.Load("block_r.png");
            var rect = Camera.ApplyCamera(this);
            g.DrawImage(sprite, rect.X, rect.Y, rect.Width, rect.Height);
        }
    }
}