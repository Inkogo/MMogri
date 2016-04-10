using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMogri
{
    class MapUpdate
    {
        public Guid mapId;
        public int x, y;

        public bool changeLight;
        public int lightLvl;

        public bool changeItem;
        public int itemId;

        public bool changeTile;
        public int tileId;

        public bool changeCovered;
        public bool covered;

        public MapUpdate(Guid mapId, int x, int y, int lightLvl = -1, int tileId = -1, int itemId = -1, int covered = -1)
        {
            this.mapId = mapId;
            this.x = x;
            this.y = y;

            if (lightLvl >= 0)
            {
                changeLight = true;
                this.lightLvl = lightLvl;
            }
            if (itemId >= 0)
            {
                changeTile = true;
                this.tileId = tileId;
            }
            if (itemId >= 0)
            {
                changeItem = true;
                this.itemId = itemId;
            }
            if (covered >= 0)
            {
                changeCovered = true;
                this.covered = covered == 0 ? false : true;
            }
        }
    }
}
