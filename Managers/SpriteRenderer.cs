using System.Drawing;

namespace MarioArcade.Managers
{
    public static class SpriteRenderer
    {
        private static readonly string SpritePath = "Sprites/";

        public static Image Load(string fileName)
        {
            try
            {
                string path = Path.Combine("Sprites", fileName);
                return Image.FromFile(path);
            }
            catch
            {
                // Если файла не найден - создать цветной прямоугольник
                var bmp = new Bitmap(32, 32);
                using (var g = Graphics.FromImage(bmp))
                {
                    // Цвет по типу объекта
                    Brush brush = fileName.Contains("player") ? Brushes.Blue :
                                 fileName.Contains("enemy") ? Brushes.Red :
                                 fileName.Contains("coin") ? Brushes.Yellow :
                                 fileName.Contains("block") ? Brushes.Gray :
                                 fileName.Contains("background") ? Brushes.LightBlue : Brushes.White;

                    g.FillRectangle(brush, 0, 0, 32, 32);
                }
                return bmp;
            }
        }
    }
}