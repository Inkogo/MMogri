using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMogri.Renderer;
using MMogri.Input;

namespace MMogri
{
    class StartScreen : ContentFrame
    {
        int menu = 0;
        int selection = 0;

        public StartScreen(GameWindow w, InputHandler i) : base(w, i) { }

        override public void Start()
        {
            DrawFrame(2, window.CenterY - 2, window.sizeX - 4, 4);
            window.Wait(.1f);
            DrawFrame(2, window.CenterY - 5, window.sizeX - 4, 10);
            window.Wait(.1f);
            DrawFrame(2, window.CenterY - 12, window.sizeX - 4, 24);
            window.Wait(.1f);
            DrawFrame(2, 2, window.sizeX - 4, window.sizeY - 4);

            DrawMainScreen();
        }

        void DrawMainScreen()
        {
            window.SetRect(title, window.CenterX - 20, 6, 40, 12);
            window.SetLine("@made by Inko (inkooognito@gmail.com) 2016", window.CenterX - 20, window.sizeY - 5);

            window.SetLine("Host Server", window.CenterX - 6, 20);
            window.SetLine("Join Server", window.CenterX - 6, 24);
            window.SetLine("Start Cmd Mode", window.CenterX - 6, 28);

            UpdateSelection();

            while (menu == 0)
            {
                input.CatchInput();
                if (input.GetKey(KeyCode.UpArrow, KeyCode.W))
                {
                    selection--;
                    if (selection < 0) selection = 2;
                    UpdateSelection();
                }
                else if (input.GetKey(KeyCode.DownArrow, KeyCode.S))
                {
                    selection++;
                    if (selection > 2) selection = 0;
                    UpdateSelection();
                }
                else if (input.GetKey(KeyCode.Enter))
                {
                    ClearFrame();
                    menu = 1;
                }
            }
        }

        void UpdateSelection()
        {
            window.SetChar(selection == 0 ? '♣' : ' ', window.CenterX - 10, 20);
            window.SetChar(selection == 1 ? '♣' : ' ', window.CenterX - 10, 24);
            window.SetChar(selection == 2 ? '♣' : ' ', window.CenterX - 10, 28);
        }

        string title = @"
___  ______  ___                  _ 
|  \/  ||  \/  |                 (_)
| .  . || .  . | ___   __ _ _ __  _
| |\/| || |\/| |/ _ \ / _` | '__|| |
| |  | || |  | | (_) | (_| | |   | |
\_|  |_/\_|  |_/\___/ \__, |_|   |_|
                       __/ |       
                      |___/        
";
    }
}
