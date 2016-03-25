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
        public List<RaceInf> raceInf;
        public List<CharacterClassInf> charaInf;
        public List<Map> maps;
        public List<Tileset> tilesets;
        public List<Item> items;
        public List<Quest> quests;

        public void Load(string path)
        {
            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
            Directory.CreateDirectory(fullPath);
            raceInf = TryLoad<RaceInf>(fullPath, "Races");
            charaInf = TryLoad<CharacterClassInf>(fullPath, "CharacterClasses");
            maps = TryLoad<Map>(fullPath, "Maps");
            tilesets = TryLoad<Tileset>(fullPath, "Tilesets");
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
    }
}
