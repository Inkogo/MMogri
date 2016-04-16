using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMogri.Renderer;

namespace MMogri
{
    public class Entity
    {
        public Guid Id;
        public string name;

        public int x;
        public int y;

        public Entity() : this("Default", 0, 0)
        { }

        public Entity(string n, int x, int y)
        {
            Id = Guid.NewGuid();
            name = n;
            this.x = x;
            this.y = y;
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

        virtual public void OnTick() { }
    }
}
