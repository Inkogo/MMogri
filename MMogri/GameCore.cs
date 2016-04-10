using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMogri.Gameplay;
using MMogri.Scripting;

namespace MMogri.Core
{
    class GameCore
    {
        GameLoader loader;
        Action<MapUpdate> updateCallback;
        //tileset, itemSet, etc

        public GameCore(GameLoader loader, Action<MapUpdate> updateCallback)
        {
            this.loader = loader;
            this.updateCallback = updateCallback;
        }

        [LuaFunc]
        public TileType CheckTileType(Guid mapId, int x, int y)
        {
            return loader.GetTileset.tileTypes[loader.GetMap(mapId)[x, y].tileType];
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
            e.x += x;
            e.y += y;
        }

        [LuaFunc]
        public void Teleport(Player p, string map, int x, int y)
        {
            p.mapId = loader.GetMap(map).Id;
            p.x = x;
            p.y = y;
        }

        [LuaFunc]
        public void TestSend (Guid mapId, int x, int y)
        {
            updateCallback(new MapUpdate(mapId, x, y, 2));
        }
    }
}
