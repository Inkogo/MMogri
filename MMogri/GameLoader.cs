﻿using System;
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
        Dictionary<string, RaceInf> raceInf;
        Dictionary<string, CharacterClassInf> charaInf;
        Dictionary<string, Map> maps;
        Dictionary<string, TileType> tileTypes;
        Dictionary<string, Item> items;
        Dictionary<string, Quest> quests;

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

 Dictionary<string, T> TryLoad<T>(string path, string sub) where T : new()
        {
       Dictionary<string,T> t = new Dictionary<string,T>();

            string fullPath = Path.Combine(path, sub);
            Directory.CreateDirectory(fullPath);

            string[] files = Directory.GetFiles(fullPath)
                             .Where(p => p.EndsWith(".xml"))
                             .ToArray();
            if (files.Length == 0)
            {
                T def = new T();
                string p = Path.Combine(fullPath, "default.xml");
                Save<T>(def, p);
                t.Add(p, def);
            }

            foreach (string p in files)
            {
                t.Add(p, FileUtils.LoadFromXml<T>(p));
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
            foreach (Map m in maps.Values)
            {
                if (check(m))
                    return m;
            }
            return null;
        }

        public void SaveMaps()
        {
            foreach (string s in maps.Keys.Where(x => maps[x].isDirty))
            {
                Debugging.Debug.Log("Saving...");
                Save<Map>(maps[s], s);
                maps[s].isDirty = false;
            }
        }

        public Tileset GetTileset
        {
            get
            {
                if (tileset == null)
                    tileset = new Tileset(tileTypes.Values.OrderBy(x => x.id).ToArray());
                return tileset;
            }
        }


    }
}
