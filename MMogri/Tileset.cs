using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MMogri.Gameplay
{
    [System.Serializable]
    public class Tileset
    {
        public TileType[] tileTypes;

        public Tileset()
        {
            tileTypes = new TileType[1];
        }

        public Tileset(params TileType[] t)
        {
            tileTypes = t;
        }

        public void WriteBytes(BinaryWriter w)
        {
            w.Write(tileTypes.Length);
            foreach (TileType t in tileTypes)
                t.WriteBytes(w);
        }

        public static Tileset FromBytes(BinaryReader r)
        {
            Tileset t = new Tileset();
            int numb = r.ReadInt32();
            t.tileTypes = new TileType[numb];
            for (int i = 0; i < numb; i++)
                t.tileTypes[i] = TileType.FromBytes(r);
            return t;
        }
    }
}
