using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MMogri.Utils;
using MMogri.Gameplay;
using MMogri.Core;

namespace MMogri
{
    class GameLoader
    {
        List<RaceInf> raceInf;
        List<CharacterClassInf> charaInf;
        List<Map> maps;
        List<TileType> tileTypes;
        List<Item> items;
        List<Quest> quests;

        Tileset tileset;

        public void Load(string fullPath)
        {
            Directory.CreateDirectory(fullPath);
            raceInf = TryLoad<RaceInf>(fullPath, "Races");
            charaInf = TryLoad<CharacterClassInf>(fullPath, "CharacterClasses");
            maps = TryLoad<Map>(fullPath, "Maps");
            tileTypes = TryLoad<TileType>(fullPath, "Tiles");
            items = TryLoad<Item>(fullPath, "Items");
            quests = TryLoad<Quest>(fullPath, "Quests");
        }

        List<T> TryLoad<T>(string path, string sub) where T : new()
        {
            List<T> t = new List<T>();

            string fullPath = Path.Combine(path, sub);
            Directory.CreateDirectory(fullPath);

            string[] files = Directory.GetFiles(fullPath)
                             .Where(p => p.EndsWith(".xml"))
                             .ToArray();
            if (files.Length == 0)
            {
                T def = new T();
                Save<T>(def, Path.Combine(fullPath, "default.xml"));
                t.Add(def);
            }

            foreach (string p in files)
            {
                t.Add(FileUtils.LoadFromXml<T>(p));
            }

            return t;
        }

        void Save<T>(T t, string path)
        {
            FileUtils.SaveToXml<T>(t, path);
        }

        public Map GetMap(Guid id)
        {
            return GetMap((Map m) => m.Id == id);
        }

        public Map GetMap(string name)
        {
            return GetMap((Map m) => m.name == name);
        }

        public Map GetMap(Func<Map, bool> check)
        {
            foreach (Map m in maps)
            {
                if (check(m))
                    return m;
            }
            return null;
        }

        public Tileset GetTileset
        {
            get
            {
                if (tileset == null)
                    tileset = new Tileset(tileTypes.OrderBy(x => x.id).ToArray());
                return tileset;
            }
        }


    }
}
