﻿using MMogri.Scripting;

namespace MMogri
{
    public class PlayerState : ScriptableObject
    {
        public string name;
        public bool adminOnly;  //replace this later!
        public string keybindPath;

        public void OnAction(string action, params object[] o)
        {
            CallLuaCallback(action, o);
        }
    }
}
