namespace MarioArcade.Managers
{
    public static class CollisionDetector
    {
        public static bool IsColliding(GameObject a, GameObject b)
        {
            if (a == null || b == null) return false;

            return a.X < b.X + b.Width && a.X + a.Width > b.X &&
                   a.Y < b.Y + b.Height && a.Y + a.Height > b.Y;
        }
    }
}