using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using MarioArcade.Data;
using MarioArcade.Objects;

namespace MarioArcade.Managers
{
    public class LevelManager
    {
        public LevelConfig Config { get; private set; }

        public float GroundY => (Config.MapHeight - 5) * Config.BlockSize; // Координаты спавна земли: 5 блоков от нижнего края камеры

        public LevelManager(string jsonFilePath)
        {
            var jsonString = File.ReadAllText(jsonFilePath);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            };

            Config = JsonSerializer.Deserialize<LevelConfig>(jsonString, options);

            // Устанавливаем фиксированные значения локации по Y и размер блока
            Config.MapHeight = 20;
            Config.BlockSize = 32;
        }

        public List<GameObject> CreateObjects()
        {

            var objects = new List<GameObject>();

            float startX = 0, startY = 0;

            foreach (var objData in Config.Objects)
            {
                float finalX = objData.X * Config.BlockSize;
                float finalY = (Config.MapHeight - 4 - objData.Y) * Config.BlockSize;

                GameObject obj = null;

                if (objData.Type == ObjectType.Enemy)
                {
                    obj = ObjectSpawner.Spawn(objData.Type, finalX, finalY, objData.MoveStart);
                }
                else if (objData.Type == ObjectType.FinishFlag)
                {
                    objData.Type = ObjectType.Flag; // Преобразование типа
                    finalX = (Config.MapWidth - 1) * Config.BlockSize; // Спавн финишнего флага в конце локации
                    obj = ObjectSpawner.Spawn(objData.Type, finalX, finalY, flagType: FlagType.Finish);
                }
                else
                {
                    obj = ObjectSpawner.Spawn(objData.Type, finalX, finalY);
                }

                if (obj != null)
                {
                    objects.Add(obj);

                    if (objData.Type == ObjectType.Player)
                    {
                        startX = finalX;
                        startY = finalY;
                    }
                }
            }

            objects.Add(new Flag { X = startX, Y = startY, Type = FlagType.Start }); // Спавн стартового флага

            return objects;
        }
    }
}