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

        List<Action> ticks;

        static void Main(string[] args)
        {
            GameMain game = new GameMain();
            game.Start();
        }

        public GameMain()
        {
            gameWindow = new GameWindow(UserPreferences.Instance.windowSizeX, UserPreferences.Instance.windowSizeY);
            input = new InputHandler();
            ticks = new List<Action>();
        }

        public void Start()
        {
            ServerMain server = new ServerMain("TestServer", gameWindow);
            MMogri.Network.NetworkHandler.Instance.StartServer(25565, server);
            ticks.Add(server.ServerTick);

            ClientMain client = new ClientMain(gameWindow, input);
            MMogri.Network.NetworkHandler.Instance.ConnectToServer(System.Net.IPAddress.Parse("192.168.178.23"), 25565, client);
            ticks.Add(client.ClientTick);

            //StartScreen s = new StartScreen(gameWindow, input);
            //s.Start();

            //GameLoader loader = new GameLoader();
            //loader.Load("TestServer");

            while (true) { }
        }
    }
}
