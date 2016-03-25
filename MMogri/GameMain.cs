using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMogri.Renderer;
using MMogri.Input;

namespace MMogri.Core
{
    class GameMain
    {
        GameWindow gameWindow;
        InputHandler input;

        static void Main(string[] args)
        {
            GameMain game = new GameMain();
            game.Start();
        }

        public GameMain ()
        {
//int x =            UserPreferences.Instance.windowSizeX;
            gameWindow = new GameWindow(70, 46);
            input = new InputHandler();
        }

        public void Start()
        {
            //StartScreen s = new StartScreen(gameWindow, input);
            //s.Start();

            //GameLoader loader = new GameLoader();
            //loader.Load("TestServer");

            //TilesetEditor t = new TilesetEditor();
            //t.Start();

            //Console.Write("⌂♫");

            //while (true) { }

            MMogri.Gameplay.Player p = new MMogri.Gameplay.Player();
            MMogri.Gameplay.Tileset t = new Gameplay.Tileset(new TileType("dirt", ' ', Color.White, false), new TileType("rock", 'F', Color.White, true));
            MMogri.Gameplay.Map m = new MMogri.Gameplay.Map("Undead Woods", 40, 40);
            m.SetTile(0, 4, 1);
            m.SetTile(1, 5, 1);
            m.SetTile(5, 8, 1);

            MapScreen s = new MapScreen(gameWindow, input, p, m);
            s.Start();

            //CmdConsole cmd = new CmdConsole();
            //while(true)
            //{
            //    string s = Console.ReadLine();
            //    cmd.ExecCmd(s);
            //}

            //            Console.WriteLine(@"
            //  ▲████▓██████████████
            //  ▲█▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█▲
            //   ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█ ▲▲
            //▲▲ █▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█▲▲▲
            //▲  █▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█▲ ░
            //   █████████████  ████░░░
            //      ▲▲▲▲░░       C░░░
            //    ░░░    P    ░░░░░  ░░░
            //");

            //while (true) { }
        }
    }
}
