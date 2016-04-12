using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MMogri.Gameplay
{
    [System.Serializable]
    public class Map
    {
        public Guid Id;
        public string name;
        public int sizeX;
        public int sizeY;

        public int baseLightLvl;

        public Tile[] tiles;
        public List<Entity> entities;

        public Map() : this("Default", 32, 32)
        { }

        public Map(string n, int x, int y)
        {
            Id = Guid.NewGuid();
            name = n;
            sizeX = x;
            sizeY = y;
            tiles = new Tile[sizeX * sizeY];
        }

        public Tile this[int x, int y]
        {
            get
            {
                if (!CheckTileBounds(x, y)) return default(Tile);
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
            SetTile(x, y, (Tile t) => t.tileType = id);
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
                    UpdateLight(x, y, t.tileTypes[this[x, y].tileType].lightEmission, ref points, t);
                }
            }
        }

        public void UpdateLightMap (int x, int y, Tileset t)
        {
            List<Point> points = new List<Point>();
            UpdateLight(x, y, t.tileTypes[this[x, y].tileType].lightEmission, ref points, t);
        }

        void UpdateLight(int x, int y, int f, ref List<Point> points, Tileset t)
        {
            if (!this[x, y].covered && f < baseLightLvl)
                f = baseLightLvl;

            Point p = new Point(x, y);
            points.Add(p);
            SetTileLight(x, y, f);
            if (f > 0 && t.tileTypes[this[x, y].tileType].translucent == true)
            {
                foreach (Direction d in Direction.Directions())
                    if (!points.Contains(p + d))
                        UpdateLight(p.x, p.y, (byte)(f - 1), ref points, t);
            }
        }

        //public void WriteBytes(BinaryWriter w)
        //{
        //    w.Write(name);
        //    w.Write(sizeX);
        //    w.Write(sizeY);

        //    foreach (Tile t in tiles)
        //    {
        //        w.Write(t.tileType);
        //        w.Write(t.itemType);
        //        w.Write(t.lightLvl);
        //        w.Write(t.covered);
        //    }
        //}

        //public static Map FromBytes(BinaryReader r)
        //{
        //    Map m = new Map(
        //        r.ReadString(),
        //        r.ReadInt32(),
        //        r.ReadInt32()
        //        );
        //    for (int i = 0; i < m.sizeX * m.sizeY; i++)
        //    {
        //        m.tiles[i] = new Tile(r.ReadInt32(), r.ReadInt32(), r.ReadInt32(), r.ReadBoolean());
        //    }
        //    return m;
        //}
    }
}
