using MarioArcade.Data;
using MarioArcade.Objects;

namespace MarioArcade.Data
{
    public static class ObjectSpawner
    {
        public static GameObject Spawn(ObjectType type, float x, float y, string moveStart = "right", FlagType flagType = FlagType.Finish)
        {
            return type switch
            {
                ObjectType.Coin => new Coin { X = x, Y = y },
                ObjectType.Block => new Block { X = x, Y = y },
                ObjectType.BlockR => new BlockR { X = x, Y = y },
                ObjectType.BlockG => new BlockG { X = x, Y = y },
                ObjectType.Ground => new Ground { X = x, Y = y },
                ObjectType.Flag => new Flag { X = x, Y = y, Type = flagType },
                ObjectType.Enemy => new Enemy { X = x, Y = y, InitialDirection = moveStart },
                ObjectType.Player => new Player { X = x, Y = y },
                _ => null
            };
        }
    }
}