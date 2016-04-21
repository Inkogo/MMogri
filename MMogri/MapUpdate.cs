using System;
using System.IO;

namespace MMogri
{
    class MapUpdate : ClientUpdate
    {
        public int x, y;

        public bool changeLight;
        public int lightLvl;

        public bool changeItem;
        public int itemId;

        public bool changeTile;
        public int tileId;

        public bool changeCovered;
        public bool covered;

        public MapUpdate(Guid mapId, int x, int y, int lightLvl = -1, int tileId = -1, int itemId = -1, int covered = -1) : base(mapId, ClientUpdateType.mapUpdate)
        {
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

        public override void WriteToBytes(BinaryWriter writer)
        {
            writer.Write(x);
            writer.Write(y);

            writer.Write(changeLight);
            if (changeLight) writer.Write(lightLvl);
            writer.Write(changeTile);
            if (changeTile) writer.Write(tileId);
            writer.Write(changeItem);
            if (changeItem) writer.Write(itemId);
            writer.Write(changeCovered);
            if (changeCovered) writer.Write(covered);
        }

        public override void ReadFromBytes(BinaryReader r)
        {
            throw new NotImplementedException();
        }

        public override void FromBytes(byte[] b)
        {
            throw new NotImplementedException();
        }
    }
}
