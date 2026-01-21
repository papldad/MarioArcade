using System.Drawing;
using MarioArcade.Managers;

namespace MarioArcade.Objects
{
    public enum FlagType
    {
        Start,  // Стартовый флаг
        Finish  // Финишный флаг
    }

    public class Flag : GameObject
    {
        public FlagType Type { get; set; }

        public override void Update()
        {
        }

        public override void Draw(Graphics g)
        {
            var rect = Camera.ApplyCamera(this);
            string spriteName = Type == FlagType.Start ? "flagstart.png" : "flagfinish.png";
            var sprite = SpriteRenderer.Load(spriteName);
            g.DrawImage(sprite, rect.X, rect.Y, rect.Width, rect.Height);
        }
    }
}