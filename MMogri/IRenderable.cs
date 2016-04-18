using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMogri.Renderer
{
    interface IRenderable
    {
        char GetTag(Tile t);
        Color GetColor(Tile t);
    }
}