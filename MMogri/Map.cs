using MMogri.Scripting;
using System;
using System.Collections.Generic;
using MMogri.Renderer;

namespace MMogri.Gameplay
{
    [System.Serializable]
    public class Map : ScriptableObject
    {
        public Guid Id;
        public string name;
        public int sizeX;
        public int sizeY;

        public int baseLightLvl;

        public Tile[] tiles;
        public List<Entity> entities;

        public string onTickCallback;
        public string onExitTopCallback;
        public string onExitRightCallback;
        public string onExitBottomCallback;
        public string onExitLeftCallback;

        [System.NonSerialized]
        public bool isDirty;

        public Map() : this("Default", 32, 32)
        { }

        public Map(string n, int x, int y)
        {
            Id = Guid.NewGuid();
            name = n;
            sizeX = x;
            sizeY = y;
            tiles = new Tile[sizeX * sizeY];
            for (int i = 0; i < sizeX * sizeY; i++)
                tiles[i] = new Tile();
        }

        public Tile this[int x, int y]
        {
            get
            {
                if (!CheckTileBounds(x, y)) return null;
                return tiles[(y * sizeX) + x];
            }
            set
            {
                if (!CheckTileBounds(x, y)) return;
                tiles[(y * sizeX) + x] = value;
            }
        }

        public void SetTileType(int x, int y, int id)
        {
            SetTile(x, y, (Tile t) => t.tileTypeId = id);
        }

        public void SetTileLight(int x, int y, int lvl)
        {
            SetTile(x, y, (Tile t) => t.lightLvl = lvl);
        }

        public void SetTile(int x, int y, Action<Tile> a)
        {
            a(tiles[(y * sizeX) + x]);
        }

        public bool CheckTileBounds(int x, int y)
        {
            return x >= 0 && x < sizeX && y >= 0 && y < sizeY;
        }

        public void UpdateLightMap(Tileset t)
        {
            List<Point> points = new List<Point>();
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    if (points.Contains(new Point(x, y))) continue;
                    UpdateLight(x, y, t.GetTileType(this[x, y].tileTypeId).lightEmission, ref points, t);
                }
            }
        }

        public void UpdateLightMap(int x, int y, Tileset t)
        {
            List<Point> points = new List<Point>();
            UpdateLight(x, y, t.GetTileType(this[x, y].tileTypeId).lightEmission, ref points, t);
        }

        public Entity FindEntityOnPosition(int x, int y)
        {
            foreach (Entity e in entities)
            {
                if (e.x == x && e.y == y) return e;
            }
            return null;
        }

        public void UpdateMapBake(Tileset tileset)
        {
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    Tile t = this[x, y];
                    //optimize this by doing the search once for all entites and store it in a quick lookup table!
                    Entity e = FindEntityOnPosition(x, y);

                    IRenderable r;
                    if (e != null)
                    {
                        r = e;
                    }
                    else if (t.itemType >= 0)
                    {
                        r = tileset.GetItem(t.itemType);
                    }
                    else
                    {
                        r = tileset.GetTileType(t.tileTypeId);
                    }

                    t.UpdateBake(r.GetTag(t), r.GetColor(t));
                }
            }
        }

        public void OnTick()
        {
            CallLuaCallback(onTickCallback, null);
        }

        public void OnExitTop(Player p)
        {
            CallLuaCallback(onExitTopCallback, new object[] { p });
        }

        public void OnExitRight(Player p)
        {
            CallLuaCallback(onExitRightCallback, new object[] { p });
        }

        public void OnExitBottom(Player p)
        {
            CallLuaCallback(onExitBottomCallback, new object[] { p });
        }

        public void OnExitLeft(Player p)
        {
            CallLuaCallback(onExitLeftCallback, new object[] { p });
        }

        void UpdateLight(int x, int y, int f, ref List<Point> points, Tileset t)
        {
            if (!this[x, y].covered && f < baseLightLvl)
                f = baseLightLvl;

            Point p = new Point(x, y);
            points.Add(p);
            SetTileLight(x, y, f);
            if (f > 0 && t.GetTileType(this[x, y].tileTypeId).translucent == true)
            {
                foreach (Direction d in Direction.Directions())
                    if (!points.Contains(p + d))
                        UpdateLight(p.x, p.y, (byte)(f - 1), ref points, t);
            }
        }
    }
}
