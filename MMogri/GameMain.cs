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

        public GameMain()
        {
            gameWindow = new GameWindow(UserPreferences.Instance.windowSizeX, UserPreferences.Instance.windowSizeY);
            input = new InputHandler();
        }

        public void Start()
        {
            CmdConsole cmd = new CmdConsole();
            while(true)
            {
                string s = Console.ReadLine();
                cmd.ExecCmd(s);
            }

            //ClientMain test = new ClientMain();
            //test.Start(gameWindow, input);

            //StartScreen s = new StartScreen(gameWindow, input);
            //s.Start();

            //GameLoader loader = new GameLoader();
            //loader.Load("TestServer");

            ////Debugging.Debug.Log((int)'c');

            //foreach (TileType t in loader.tileTypes.OrderBy(x => x.id))
            //    Debugging.Debug.Log(t.tag + ", " + t.name + ", " + t.id);
            //TilesetEditor t = new TilesetEditor();
            //t.Start();

            //Console.Write("⌂♫");

            while (true) { }

            //MMogri.Gameplay.Player p = new MMogri.Gameplay.Player();
            //MMogri.Gameplay.Tileset t = new Gameplay.Tileset(new TileType("dirt", ' ', Color.White, false), new TileType("rock", 'F', Color.White, true));
            //MMogri.Gameplay.Map m = new MMogri.Gameplay.Map("Undead Woods", 40, 40);
            //m.SetTile(0, 4, 1);
            //m.SetTile(1, 5, 1);
            //m.SetTile(5, 8, 1);

            //MapScreen s = new MapScreen(gameWindow, input, p, m);
            //s.Start();

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
