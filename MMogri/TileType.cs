using MMogri.Renderer;
using MMogri.Scripting;

namespace MMogri
{
    [System.Serializable]
    public class TileType : ScriptableObject, IRenderable
    {
        public byte index;
        public string name;

        public char tag;
        public Color color;

        public byte lightEmission;

        public bool solid;
        public bool translucent;
        public bool flameable;

        public string getTagCallback;
        public string getColorCallback;

        public TileType () { }

        public TileType(byte id, string name, char tag, Color color, bool solid, bool translucent, byte lightEmission)
        {
            this.index = id;
            this.name = name;

            this.tag = tag;
            this.color = color;

            this.solid = solid;
            this.translucent = translucent;
            this.lightEmission = lightEmission;
        }

        public char GetTag(Tile t)
        {
            object[] o = CallLuaCallback(getTagCallback, new object[] { t });
            if (o != null && o.Length == 1)
            {
                return (char)o[0];
            }
            return tag;
        }

        public Color GetColor(Tile t)
        {
            object[] o = CallLuaCallback(getColorCallback, new object[] { t });
            if (o != null && o.Length == 1)
            {
                return (Renderer.Color)o[0];
            }
            return color;
        }
    }
}
