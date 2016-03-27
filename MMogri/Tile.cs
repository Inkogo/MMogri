using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMogri
{
    [System.Serializable]
    public struct Tile
    {
       public int tileType;
        //int itemType;

        //public char tag;
        //public Renderer.Color tagColor;

        //public byte lightLvl;

        public Tile(int t)
        {
            tileType = t;
        }

        public void SetTileType (int i)
        {
            tileType = i;
        }
    }
}
