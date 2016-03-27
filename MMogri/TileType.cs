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
        public byte id;
        public string name;
        public char tag;
        public Color tagColor;

        public bool solid;
        public byte lightEmission;

        public TileType(byte id, string name, char tag, Color tagColor, bool solid, byte lightEmission)
        {
            this.id = id;
            this.name = name;
            this.tag = tag;
            this.tagColor = tagColor;
            this.solid = solid;
            this.lightEmission = lightEmission;
        }

        public void WriteBytes(BinaryWriter w)
        {
            w.Write(id);
            w.Write(name);
            w.Write(tag);
            w.Write((int)tagColor);
            w.Write(solid);
            w.Write(lightEmission);
        }

        public static TileType FromBytes(BinaryReader r)
        {
            TileType t = new TileType(
                r.ReadByte(),
                r.ReadString(),
                r.ReadChar(),
                (Color)r.ReadInt32(),
                r.ReadBoolean(),
                r.ReadByte()
                );
            return t;
        }
    }
}
