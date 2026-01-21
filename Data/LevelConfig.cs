using System.Collections.Generic;
using MarioArcade.Data;

namespace MarioArcade.Data
{
    public class LevelConfig
    {
        public int MapWidth { get; set; }
        public int MapHeight { get; set; }
        public float BlockSize { get; set; }
        public List<ObjectData> Objects { get; set; } = new();
    }

    public class ObjectData
    {
        public ObjectType Type { get; set; }
        public int X { get; set; } = 0;
        public int Y { get; set; } = 0;
        public string MoveStart { get; set; }
    }
}