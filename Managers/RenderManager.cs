using System.Drawing;
using MarioArcade.Managers;
using MarioArcade.Objects;

namespace MarioArcade.Managers
{
    public class RenderManager
    {
        private Image _background;
        private int _mapWidth;
        private LevelManager _levelManager;

        public RenderManager(LevelManager levelManager, Image background, int mapWidth)
        {
            _levelManager = levelManager;
            _background = background;
            _mapWidth = mapWidth;
        }

        public void Draw(Graphics g, float cameraX)
        {
            var gameSurface = new Bitmap(800, 600);
            using (var gameGraphics = Graphics.FromImage(gameSurface))
            {
                if (_background != null)
                {
                    float bgX = -cameraX * 0.5f;
                    float bgY = 0;
                    int renderWidth = 800;
                    int width = _background.Width;

                    // Формула отображения фона: фон * 1 + размер_локации + фон * 1
                    int totalRenderWidth = (width * 1) + _mapWidth + (width * 1);

                    for (int x = (int)bgX - width; x < bgX + totalRenderWidth; x += width)
                    {
                        gameGraphics.DrawImage(_background, x, bgY, width, 600);
                    }
                }
                else
                {
                    gameGraphics.FillRectangle(Brushes.LightBlue, 0, 0, 800, 600);
                }

                int blockSize = 32;
                float startY = _levelManager.GroundY + blockSize;

                float visibleStart = cameraX - blockSize;
                float visibleEnd = cameraX + 800 + blockSize;

                for (float x = visibleStart; x <= visibleEnd; x += blockSize)
                {
                    float screenX = x - cameraX;

                    try
                    {
                        var sprite = SpriteRenderer.Load("ground.png");
                        gameGraphics.DrawImage(sprite, screenX, startY, blockSize, blockSize);
                    }
                    catch
                    {
                        gameGraphics.FillRectangle(Brushes.Green, screenX, startY, blockSize, blockSize);
                    }
                }

                // Ограничиваем отрисовку только видимыми объектами для производительности
                float visibleMinX = cameraX - 32;
                float visibleMaxX = cameraX + 800 + 32;

                foreach (var obj in GameForm.CurrentGameObjects)
                {
                    if (obj is Ground) continue;

                    if (obj.X + obj.Width < visibleMinX ||
                        obj.X > visibleMaxX)
                    {
                        continue;
                    }

                    obj.Draw(gameGraphics);
                }
            }

            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            g.DrawImage(gameSurface, 0, 0, 800, 600);

            gameSurface.Dispose();
        }
    }
}