using MMogri.Gameplay;
using MMogri.Renderer;
using MMogri.Scripting;
using System;

namespace MMogri
{
    [System.Serializable]
    public class Entity : ScriptableObject
    {
        public Guid Id;
        public string name;

        public int x;
        public int y;

        public string OnTickCallback;
        public string OnInteractCallback;

        public Entity() : this("Default", 0, 0)
        { }

        public Entity(string n, int x, int y) : base()
        {
            Id = Guid.NewGuid();
            name = n;
            this.x = x;
            this.y = y;
        }

        public void OnInteract(Player p)
        {
            CallLuaCallback(OnInteractCallback, p);
        }

        public void OnTick()
        {
            CallLuaCallback(OnTickCallback, null);
        }

        virtual public char Tag
        {
            get
            {
                return ' ';
            }
        }

        virtual public Color TagColor
        {
            get
            {
                return Color.White;
            }
        }
    }
}
