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
        public int x;
        public int y;
        public string name;

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
