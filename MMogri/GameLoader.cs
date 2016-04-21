using MMogri.Gameplay;
using MMogri.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
        Dictionary<string, PlayerState> playerStates;
        Dictionary<string, Keybind[]> keybinds;

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
            playerStates = TryLoad<PlayerState>(fullPath, "PlayerStates");
            //keybinds = TryLoad<Keybind[]>(fullPath, "Keybinds");

            Debugging.Debug.Log("AAAAH");
        }

        Dictionary<string, T> TryLoad<T>(string path, string sub) where T: new()
        {
            Dictionary<string, T> t = new Dictionary<string, T>();

            string fullPath = Path.Combine(path, sub);
            Directory.CreateDirectory(fullPath);

            string[] files = Directory.GetFiles(fullPath)
                             .Where(p => p.EndsWith(".mog"))
                             .ToArray();
            if (files.Length == 0)
            {
                T def = new T();
                string p = Path.Combine(fullPath, "default.mog");
                Save<T>(def, p);
                t.Add(p, def);
            }

            foreach (string p in files)
            {
                //t.Add(p, FileUtils.LoadFromXml<T>(p));
                t.Add(p, FileUtils.LoadFromMog<T>(p));
            }

            return t;
        }

        void Save<T>(T t, string path) where T : new()
        {
            FileUtils.SaveToMog<T>(t, path);
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

        public bool SaveMaps()
        {
            bool saved = false;
            foreach (string s in maps.Keys.Where(x => maps[x].isDirty))
            {
                Save<Map>(maps[s], s);
                maps[s].isDirty = false;
                saved = true;
            }
            return saved;
        }

        public Tileset GetTileset
        {
            get
            {
                if (tileset == null)
                    tileset = new Tileset(tileTypes.Values.ToArray(), items.Values.ToArray());
                return tileset;
            }
        }

        public PlayerState GetPlayerState(string s)
        {
            foreach (PlayerState p in playerStates.Values)
            {
                if (p.name == s) return p;
            }
            return null;
        }

        public Keybind[] GetKeybindsByPath (string p)
        {
            return keybinds[p];
        }
    }
}
