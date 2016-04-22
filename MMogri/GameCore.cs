using MMogri.Gameplay;
using MMogri.Scripting;
using System;
using System.Text;

namespace MMogri.Core
{
    class GameCore
    {
        GameLoader loader;
        Action<ClientUpdate> updateCallback;

        public GameCore(GameLoader loader, Action<ClientUpdate> updateCallback)
        {
            this.loader = loader;
            this.updateCallback = updateCallback;
        }

        [LuaFunc]
        public TileType CheckTileType(Guid mapId, int x, int y)
        {
            return loader.GetTileset.GetTileType(loader.GetMap(mapId)[x, y].tileTypeId);
        }

        [LuaFunc]
        public bool CheckCollision(Guid mapId, int x, int y)
        {
            return !loader.GetMap(mapId).CheckTileBounds(x, y) || CheckTileType(mapId, x, y).solid;
        }

        [LuaFunc]
        public void SetTile(Guid mapId, int x, int y, int tileId)
        {
            loader.GetMap(mapId).SetTileType(x, y, tileId);
            updateCallback(new MapUpdate(mapId, x, y, -1, tileId));
        }

        [LuaFunc]
        public void MoveEntity(Entity e, Guid mapId, int x, int y)
        {
            if (!CheckCollision(mapId, e.x - 1, e.y))
            {
                e.x += x;
                e.y += y;
            }
            updateCallback(new EntityUpdate(mapId, e.Id, x, y));
        }

        [LuaFunc]
        public void Teleport(Player p, string map, int x, int y)
        {
            Guid oldMap = p.mapId;
            p.mapId = loader.GetMap(map).Id;
            p.x = x;
            p.y = y;
            updateCallback(new MapTransitionUpdate(p.mapId, oldMap, p.Id));
        }

        [LuaFunc]
        public void SetPlayerState(Player p, string state)
        {
            p.playerState = state;
            updateCallback(new PlayerUpdate(p.mapId, p.Id, state));
        }

        [LuaFunc]
        public bool ReadScriptableBool(ScriptableDataContainer o, string s)
        {
            if (o.data.ContainsKey(s))
                return BitConverter.ToBoolean(o.data[s], 0);
            return false;
        }

        [LuaFunc]
        public void WriteScriptableBool(ScriptableDataContainer o, string s, bool b)
        {
            if (o.data.ContainsKey(s))
                o.data[s] = BitConverter.GetBytes(b);
            else
                o.data.Add(s, BitConverter.GetBytes(b));
        }

        [LuaFunc]
        public string ReadScriptableString(ScriptableDataContainer o, string s)
        {
            if (o.data.ContainsKey(s))
                return BitConverter.ToString(o.data[s], 0);
            return "";
        }

        [LuaFunc]
        public void WriteScriptableString(ScriptableDataContainer o, string s, string v)
        {
            if (o.data.ContainsKey(s))
                o.data[s] = Encoding.ASCII.GetBytes(v);
            else
                o.data.Add(s, Encoding.ASCII.GetBytes(v));
        }

        [LuaFunc]
        public int ReadScriptableInt(ScriptableDataContainer o, string s)
        {
            if (o.data.ContainsKey(s))
                return BitConverter.ToInt32(o.data[s], 0);
            return 0;
        }

        [LuaFunc]
        public void WriteScriptableInt(ScriptableDataContainer o, string s, int i)
        {
            if (o.data.ContainsKey(s))
                o.data[s] = BitConverter.GetBytes(i);
            else
                o.data.Add(s, BitConverter.GetBytes(i));
        }

        [LuaFunc]
        public void InteractWithEnity(Player p, Entity e)
        {
            e.OnInteract(p);
        }
    }
}
