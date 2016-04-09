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

        public char tagLit;
        public Color tagColorLit;

        public char tagDark;
        public Color tagColorDark;

        public bool solid;
        public bool translucent;
        public byte lightEmission;

        public TileType(byte id, string name, char tagLit, Color tagColorLit, char tagDark, Color tagColorDark, bool solid, bool translucent, byte lightEmission)
        {
            this.id = id;
            this.name = name;

            this.tagLit = tagLit;
            this.tagColorLit = tagColorLit;

            this.tagDark = tagDark;
            this.tagColorDark = tagColorDark;

            this.solid = solid;
            this.translucent = translucent;
            this.lightEmission = lightEmission;
        }

        public Color GetColor(int lightLvl)
        {
            if (lightLvl <= 8) return tagColorDark;
            else return tagColorLit;
        }

        public char GetTag (int lightLvl)
        {
            if (lightLvl <= 2) return ' ';
            if (lightLvl <= 8) return tagDark;
            else return tagLit;
        }

        public void WriteBytes(BinaryWriter w)
        {
            w.Write(id);
            w.Write(name);

            w.Write(tagLit);
            w.Write((int)tagColorLit);
            w.Write(tagDark);
            w.Write((int)tagColorDark);

            w.Write(solid);
            w.Write(translucent);
            w.Write(lightEmission);
        }

        public static TileType FromBytes(BinaryReader r)
        {
            TileType t = new TileType(
                r.ReadByte(),
                r.ReadString(),

                r.ReadChar(),
                (Color)r.ReadInt32(),
                r.ReadChar(),
                (Color)r.ReadInt32(),

                r.ReadBoolean(),
                r.ReadBoolean(),
                r.ReadByte()
                );
            return t;
        }
    }
}
