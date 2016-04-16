using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMogri.Input;

namespace MMogri
{
    [System.Serializable]
 public   struct Keybind
    {
        public KeyCode key;
        public KeyCode altKey;
        public string action;

        public Keybind(string action, KeyCode key) : this(action, key, KeyCode.NoName) { }

        public Keybind (string action, KeyCode key, KeyCode alt)
        {
            this.action = action;
            this.key = key;
            this.altKey = alt;
        }
    }
}
