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
        public int itemType;

        public int lightLvl;
        public bool covered;

        public Tile(int t, int i, int l, bool c)
        {
            tileType = t;
            itemType = i;
            lightLvl = l;
            covered = c;

        }

        public void SetTileType(int i)
        {
            tileType = i;
        }

        public void OnTick() { }
    }
}
