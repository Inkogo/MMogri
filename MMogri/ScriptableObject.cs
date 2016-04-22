namespace MMogri.Scripting
{
    [System.Serializable]
    public abstract class ScriptableObject : ScriptableDataContainer
    {
        public string luaPath;
        LuaScript lua;

        protected object[] CallLuaCallback(string s, params object[] o)
        {
            if (s != null && s != "" && luaPath != null)
            {
                if (lua == null)
                {
                    lua = new LuaScript(luaPath);
                }
                return lua.CallFunc(s, o);
            }
            return null;
        }
    }
}
