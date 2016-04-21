namespace MMogri.Scripting
{
    [System.Serializable]
    public abstract class ScriptableObject : ScriptableDataContainer
    {
        public string luaPath;
        LuaScript lua;

        public ScriptableObject()
        {
            if (luaPath != null)
            {
                lua = new LuaScript(luaPath);
            }
        }

        protected object[] CallLuaCallback(string s, params object[] o)
        {
            if (lua != null && s != null && s != "")
              return  lua.CallFunc(s, o);
            return null;
        }
    }
}
