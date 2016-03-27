using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMogri.Renderer;
using MMogri.Input;
using MMogri.Gameplay;

namespace MMogri
{
    class MapScreen : ContentFrame
    {
        Player player;
        Map map;
        Tileset tileset;

        public MapScreen(GameWindow w, InputHandler i, Player p) : base(w, i)
        {
            player = p;
        }

        public override void Start()
        {
        }

        public void Start(Map m, Tileset t)
        {
            map = m;
            tileset = t;

            DrawFrame(1, 2, window.sizeX - 20, window.sizeY - 4);

            window.SetLine(map.name, posX + width - 5 - map.name.Length, posY);

            UpdateMap();
        }

        public void UpdateMap()
        {
            for (int y = 0; y < height - 2; y++)
            {
                window.SetPosition(posX + 1, height - y);
                for (int x = 0; x < width - 2; x++)
                {
                    int x0 = player.x - (int)(width * .5f) + x;
                    int y0 = player.y - (int)(height * .5f) + y;

                    if (x0 == player.x && y0 == player.y) {
                        window.SetNext('P');
                    }
                    else
                    {
                        if (map.CheckTileBounds(x0, y0))
                        {
                            Tile t = map[x0, y0];
                            TileType tt = tileset.tileTypes[t.tileType];
                            window.SetNext(tt.tag);
                        }
                        else {
                            window.SetNext(' ');
                        }
                    }
                }
            }
        }
    }
}