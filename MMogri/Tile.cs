using MMogri.Scripting;

namespace MMogri
{
    [System.Serializable]
    public class Tile : ScriptableObject
    {
        public int tileTypeId;
        public int itemType;

        public int lightLvl;
        public bool covered;

        [System.NonSerialized]
        public char bakedTag;
        [System.NonSerialized]
        public short bakedColor;

        public string onTickCallback;
        public string onEnterCallback;
        public string onExitCallback;

        public Tile(int t, int i, int l, bool c)
        {
            tileTypeId = t;
            itemType = i;
            lightLvl = l;
            covered = c;

        }

        public void SetTileType(int i)
        {
            tileTypeId = i;
        }

        public void UpdateBake(char t, MMogri.Renderer.Color c)
        {
            bakedTag = t;
            bakedColor = (short)c;
        }

        public void OnTick()
        {
            CallLuaCallback(onTickCallback, null);
        }

        public void OnEnter(Entity e)
        {
            CallLuaCallback(onEnterCallback, new object[] { e });
        }

        public void OnExit(Entity e)
        {
            CallLuaCallback(onExitCallback, new object[] { e });
        }
    }
}
