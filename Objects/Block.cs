using System.Drawing;
using MarioArcade.Managers;

namespace MarioArcade.Objects
{
    public class Block : GameObject // Серый блок
    {
        public bool IsSolid { get; set; } = true; // Можно ли стоять на блоке

        public override void Update() { }
        public override void Draw(Graphics g)
        {
            var rect = Camera.ApplyCamera(this);
            var sprite = SpriteRenderer.Load("block.png");
            g.DrawImage(sprite, rect.X, rect.Y, rect.Width, rect.Height);
        }
    }
}