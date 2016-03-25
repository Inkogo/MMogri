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

        public MapScreen(GameWindow w, InputHandler i, Player p, Map m) : base(w, i)
        {
            player = p;
            map = m;
        }

        public override void Start()
        {
            DrawFrame(1, 2, window.sizeX - 20, window.sizeY - 4);

            window.SetLine(map.name, posX + width - 5 - map.name.Length, posY);

            UpdateMap();

            while (true)
            {
                input.CatchInput();
                if (input.GetKey(KeyCode.W) && !map.CheckCollision(player.x, player.y + 1))
                {
                    player.y += 1;
                    System.Threading.Thread.Sleep(100);
                    //player.Move(map);
                    UpdateMap();
                }
                if (input.GetKey(KeyCode.S) && !map.CheckCollision(player.x, player.y - 1))
                {
                    player.y -= 1;
                    System.Threading.Thread.Sleep(100);
                    //player.Move(map);
                    UpdateMap();
                }
                if (input.GetKey(KeyCode.A) && !map.CheckCollision(player.x - 1, player.y))
                {
                    player.x -= 1;
                    System.Threading.Thread.Sleep(100);
                    //player.Move(map);
                    UpdateMap();
                }
                if (input.GetKey(KeyCode.D) && !map.CheckCollision(player.x + 1, player.y))
                {
                    player.x += 1;
                    System.Threading.Thread.Sleep(100);
                    //player.Move(map);
                    UpdateMap();
                }
                if (input.GetKey(KeyCode.R))
                {
                    TextScreen txt = new TextScreen(window, input);
                    txt.Start();
                }
            }
        }

        void UpdateMap()
        {
            for (int y = 0; y < height - 2; y++)
            {
                window.SetPosition(posX + 1, height - y);
                for (int x = 0; x < width - 2; x++)
                {
                    int x0 = player.x - (int)(width * .5f) + x;
                    int y0 = player.y - (int)(height * .5f) + y;

                    Tile t = map[x0, y0];

                    window.SetNext(x0 == player.x && y0 == player.y ? "P" : (t.tileType == 0 ? " " : "▲"));
                }
            }
        }
    }
}