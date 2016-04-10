using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMogri.Gameplay;

namespace MMogri
{
    public class ClientGameState
    {
        public class TileVis
        {
            public char tag;
            public short color;
        }

        public string mapName;
        public int mapSizeX, mapSizeY;
        public Player player;
        public TileVis[] tiles;

        public TileVis GetMapTile(int x, int y)
        {
            return tiles[0];        //add this later!
        }
    }
}
