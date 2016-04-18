using MMogri.Input;
using MMogri.Renderer;
using System;
using System.Collections.Generic;
using System.IO;

namespace MMogri.Core
{
    class GameMain
    {
        public enum LaunchMode
        {
            server, client, debugMode
        }
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
            //Entity e = new Entity("test", 3, 3);
            //Serialization.Serializer.Serialize<Entity>(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test.xml"), e);
            //Entity e = Utils.Serializer.Deserialize<Entity>(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test.xml"));

            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test.xml");

            Serialization.TestClass t = new Serialization.TestClass(
                4,
                "bam",
                new List<string>() { "aaah", "beeeh", "ceeeeh" },
                3,
                new Dictionary<string, int>() { { "a", 1 }, { "b", 2 } },
                new Serialization.TestStruct(8)
                );
            Serialization.SerializeWriter w = new Serialization.SerializeWriter();
            w.Serialize<Serialization.TestClass>(path, t);

            //Serialization.TestClass t = Serialization.Serializer.Deserialize<Serialization.TestClass>(path);
            //Debugging.Debug.Log(t);

            return;

            // i really dont like this!
            StartScreen startScreen = new StartScreen(gameWindow, input);
            LaunchMode m = (LaunchMode)startScreen.ShowScreen();

            ServerMain server;
            ClientMain client;

            switch (m)
            {
                case LaunchMode.server:
                    server = InitServer();
                    ticks.Add(server.ServerTick);
                    CmdConsole cmd = new CmdConsole();
                    break;
                case LaunchMode.client:
                    client = InitClient();
                    ticks.Add(client.ClientTick);
                    break;
                case LaunchMode.debugMode:
                    server = InitServer();
                    ticks.Add(server.ServerTick);
                    client = InitClient();
                    ticks.Add(client.ClientTick);
                    break;
            }

            while (true)
            {
                foreach (Action a in ticks)
                    a();
                System.Threading.Thread.Sleep(10);
            }
        }

        ServerMain InitServer()
        {
            FileBrowserScreen s = new FileBrowserScreen(gameWindow, input, "serverInf.xml");
            string path = s.BrowseDirectory(AppDomain.CurrentDomain.BaseDirectory);

            string serverPath = System.IO.Path.Combine(path);

            ServerInf inf = Utils.FileUtils.LoadFromXml<ServerInf>(serverPath);
            ServerMain server = new ServerMain(inf, gameWindow);
            //MMogri.Network.NetworkHandler.Instance.StartServer(25565, server);
            //ticks.Add(server.ServerTick);
            gameWindow.Clear();

            return server;
        }

        ClientMain InitClient()
        {
            Console.WriteLine("[F] Load from File");
            Console.WriteLine("[D] Join Direct");

            ClientInf inf = null;
            string path = null;

            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.F)
                {
                    gameWindow.Clear();

                    FileBrowserScreen f = new FileBrowserScreen(gameWindow, input, "clientInf.xml");
                    path = f.BrowseDirectory(AppDomain.CurrentDomain.BaseDirectory);
                    inf = Utils.FileUtils.LoadFromXml<ClientInf>(path);

                    break;
                }
                else if (key.Key == ConsoleKey.D)
                {
                    Console.WriteLine("Ip:");
                    string ip = Console.ReadLine();
                    Console.WriteLine("Port:");
                    string port = Console.ReadLine();
                    Console.WriteLine("Name:");
                    string name = Console.ReadLine();

                    string p = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, name);
                    path = Path.Combine(p, "clientInf.xml");
                    inf = new ClientInf(ip, int.Parse(port), name);
                    Directory.CreateDirectory(p);
                    Utils.FileUtils.SaveToXml<ClientInf>(inf, path);

                    break;
                }
            }

            ClientMain client = new ClientMain(inf, gameWindow, input);

            gameWindow.Clear();

            return client;
        }
    }
}
