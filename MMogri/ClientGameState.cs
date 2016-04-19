using MMogri.Gameplay;
using System.IO;

namespace MMogri
{
    public class ClientGameState : ICompressable
    {
        public class TileVis
        {
            public bool covered;
            public int lightLvl;
            public char tag;
            public short color;

            public TileVis(bool c, int l, char t, short o)
            {
                covered = c;
                lightLvl = l;
                tag = t;
                color = o;
            }
        }

        public string mapName;
        public int mapSizeX, mapSizeY;
        public int posX, posY;
        public string posText;
        public string playerState;
        public TileVis[] tiles;

        public ClientGameState() { }

        public ClientGameState(Map m, int px, int py, string pt, string ps)
        {
            mapName = m.name;
            mapSizeX = m.sizeX;
            mapSizeY = m.sizeY;
            posX = px;
            posY = py;
            posText = pt;
            playerState = ps;

            tiles = new TileVis[mapSizeX * mapSizeY];
            for (int i = 0; i < tiles.Length; i++)
            {
                tiles[i] = new TileVis(
                    m.tiles[i].covered,
                    m.tiles[i].lightLvl,
                    m.tiles[i].bakedTag,
                    m.tiles[i].bakedColor
                );
            }
        }

        public TileVis GetMapTile(int x, int y)
        {
            if (!CheckTileBounds(x, y)) return null;
            return tiles[(y * mapSizeX) + x];
        }

        public bool CheckTileBounds(int x, int y)
        {
            return x >= 0 && x < mapSizeX && y >= 0 && y < mapSizeY;
        }

        public byte[] ToBytes()
        {
            using (MemoryStream mem = new MemoryStream())
            {
                using (BinaryWriter w = new BinaryWriter(mem))
                {
                    w.Write(mapName);
                    w.Write(mapSizeX);
                    w.Write(mapSizeY);
                    w.Write(posX);
                    w.Write(posY);
                    w.Write(posText);
                    w.Write(playerState);
                    foreach (TileVis t in tiles)
                    {
                        w.Write(t.covered);
                        w.Write(t.lightLvl);
                        w.Write(t.tag);
                        w.Write(t.color);
                    }
                }
                return mem.ToArray();
            }
        }

        public void FromBytes(byte[] b)
        {
            //client uppack code goes here!
            using (MemoryStream mem = new MemoryStream(b))
            {
                using (BinaryReader reader = new BinaryReader(mem))
                {
                    mapName = reader.ReadString();
                    mapSizeX = reader.ReadInt32();
                    mapSizeY = reader.ReadInt32();
                    // .... and so on
                }
            }
        }
    }
}