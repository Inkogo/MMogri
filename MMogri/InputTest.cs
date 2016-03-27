using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMogri.Gameplay;
using MMogri.Core;
using MMogri.Scripting;
using MMogri.Input;
using System.IO;
using MMogri.Utils;
using MMogri.Renderer;

namespace MMogri
{
    class InputTest
    {
        public void Start(GameWindow window)
        {
            //Map m = new Map("Test Map", 10, 10);
            //m.SetTile(2, 2, 2);

            //FileUtils.SaveToXml<Map>(m, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestServer", "Maps", "testMap.xml"));

            GameLoader loader = new GameLoader();
            loader.Load("TestServer");

            GameCore core = new GameCore(loader);
            LuaHandler lua = new LuaHandler();
            InputHandler input = new InputHandler();
            lua.RegisterObserved("Core", core);

            Player p = new Player();
            p.mapId = loader.GetMap("Test Map").Id;
            p.x = 2;
            p.y = 3;

            Keybind[] keybinds = FileUtils.LoadFromXml<Keybind[]>(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestServer", "keybindings.xml"));

            MapScreen s = new MapScreen(window, input, p);

            s.Start(loader.GetMap(p.mapId), loader.GetTileset);

            //Keybind[] keybinds = new Keybind[] {
            //    new Keybind(KeyCode.W, KeyCode.UpArrow, "GoNorth"),
            //    new Keybind(KeyCode.S, KeyCode.DownArrow, "GoSouth"),
            //    new Keybind(KeyCode.A, KeyCode.LeftArrow, "GoWest"),
            //    new Keybind(KeyCode.D, KeyCode.RightArrow, "GoEast"),

            //    new Keybind(KeyCode.L, "Look"),
            //};
            //FileUtils.SaveToXml<Keybind[]>(keybinds, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestServer", "keybindings.xml"));

            while (true)
            {
                input.CatchInput();
                foreach (Keybind b in keybinds)
                {
                    if (input.GetKey(b.key, b.altKey))
                    {
                        lua.CallFunc(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestServer", "testLua.lua"), b.action, p);
                        //Debugging.Debug.Log("Player: " + p.x + ", " + p.y);
                        s.UpdateMap();
                    }
                }
            }
        }
    }
}
