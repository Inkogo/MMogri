using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

//this is a really bad namescace for this! What was I thinking?
namespace MMogri.Gameplay
{
    [System.Serializable]
    public class Tileset
    {
        Dictionary<int, TileType> tileTypes;
        Dictionary<int, Item> items;

        public Tileset()
        {
            tileTypes = new Dictionary<int, TileType>();
            items = new Dictionary<int, Item>();
        }

        public Tileset(TileType[] t, Item[] i)
        {
            tileTypes = t.ToDictionary(
                x => (int)x.index,
                x => x);

            items = i.ToDictionary(
                x => (int)x.index,
                x => x);
        }

        public TileType GetTileType(int i)
        {
            if (tileTypes.ContainsKey(i))
                return tileTypes[i];
            return null;
        }

        public Item GetItem(int i)
        {
            if (items.ContainsKey(i))
                return items[i];
            return null;
        }
    }
}
