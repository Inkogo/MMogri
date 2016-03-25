using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMogri.Input;
using MMogri.Renderer;

namespace MMogri
{
    class TextScreen : ContentFrame
    {
        public TextScreen(GameWindow w, InputHandler i) : base(w, i)
        { }

        override public void Start()
        {
            SetFrame(6, window.sizeY - 10, window.sizeX - 12, 7);

            window.SetLine("Farmer Joe", posX + 4, posY);
            window.SetRect("Hey there traveler! Nice day, itn't it?", posX + 1, posY + 1, width - 2, height - 2);
        }
    }
}
