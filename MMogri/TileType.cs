using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMogri.Renderer;
using System.IO;

namespace MMogri
{
    [System.Serializable]
    public struct TileType
    {
        public string name;
        public char tag;
        public Color tagColor;
        public bool solid;

        public TileType(string name, char tag, Color tagColor, bool solid)
        {
            this.name = name;
            this.tag = tag;
            this.tagColor = tagColor;
            this.solid = solid;
        }

        public void WriteBytes(BinaryWriter w)
        {
            w.Write(name);
            w.Write(tag);
            w.Write((int)tagColor);
            w.Write(solid);
        }

        public static TileType FromBytes(BinaryReader r)
        {
            TileType t = new TileType(
                r.ReadString(),
                r.ReadChar(),
                (Color)r.ReadInt32(),
                r.ReadBoolean()
                );
            return t;
        }
    }
}
