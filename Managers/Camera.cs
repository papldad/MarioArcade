using System.Drawing;
using MarioArcade.Objects;

namespace MarioArcade.Managers
{
    public static class Camera
    {
        public static float X { get; set; } = 0;
        private static Player _player;
        private static int _mapWidth;

        public static void Initialize(Player player, int mapWidth)
        {
            _player = player;
            _mapWidth = mapWidth;
        }

        public static void Update()
        {
            if (_player == null) return;

            // Двигаем камеру за игроком
            if (_player.X > 400)
                X = _player.X - 400;

            // Ограничиваем камеру
            if (X < 0)
                X = 0;
            if (X > _mapWidth - 800)
                X = _mapWidth - 800;
        }

        public static RectangleF ApplyCamera(GameObject obj)
        {
            return new RectangleF(obj.X - X, obj.Y, obj.Width, obj.Height);
        }

        public static PointF ApplyCamera(PointF point)
        {
            return new PointF(point.X - X, point.Y);
        }
    }
}