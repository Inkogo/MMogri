using MMogri.Scripting;
using MMogri.Renderer;

namespace MMogri.Gameplay
{
    public class Item : ScriptableObject, IRenderable
    {
        public int index;
        public string name;

        public byte maxStack;
        public int weight;

        public char tag;
        public Color color;

        public string onUseCallback;
        public string onPickupCallback;
        public string onDropCallback;
        public string getTagCallback;
        public string getColorCallback;

        public void OnUse(Player p)
        {
            CallLuaCallback(onUseCallback, new object[] { p });
        }

        public void OnPickup(Player p)
        {
            CallLuaCallback(onPickupCallback, new object[] { p });
        }

        public void OnDrop(Player p)
        {
            CallLuaCallback(onDropCallback, new object[] { p });
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
