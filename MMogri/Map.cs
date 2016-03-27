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
        public Tile[] tiles;

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

        public void SetTile(int x, int y, int id)
        {
            tiles[(y * sizeX) + x].tileType = id;
        }

        public bool CheckTileBounds(int x, int y)
        {
            return x >= 0 && x < sizeX && y >= 0 && y < sizeY;
        }

        public void WriteBytes(BinaryWriter w)
        {
            w.Write(name);
            w.Write(sizeX);
            w.Write(sizeY);

            foreach (Tile t in tiles)
                w.Write(t.tileType);
        }

        public static Map FromBytes(BinaryReader r)
        {
            Map m = new Map(
                r.ReadString(),
                r.ReadInt32(),
                r.ReadInt32()
                );
            for (int i = 0; i < m.sizeX * m.sizeY; i++)
            {
                m.tiles[i] = new Tile(r.ReadInt32());
            }
            return m;
        }
    }
}
