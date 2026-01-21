using System.Drawing;

namespace MarioArcade
{
    public abstract class GameObject
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; } = 32;
        public float Height { get; set; } = 32;

        public abstract void Update();
        public abstract void Draw(Graphics g);
    }
}