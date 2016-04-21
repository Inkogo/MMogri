using MMogri.Input;
using MMogri.Renderer;

namespace MMogri
{
    class StartScreen : ContentFrame
    {
        int selection = 0;

        public StartScreen(GameWindow w, InputHandler i) : base(w, i) { }

        override public void Start()
        {

        }

        public int ShowScreen()
        {
            DrawFrame(2, window.CenterY - 2, window.sizeX - 4, 4);
            window.Wait(.1f);
            DrawFrame(2, window.CenterY - 5, window.sizeX - 4, 10);
            window.Wait(.1f);
            DrawFrame(2, window.CenterY - 12, window.sizeX - 4, 24);
            window.Wait(.1f);
            DrawFrame(2, 2, window.sizeX - 4, window.sizeY - 4);

            window.SetRect(title, window.CenterX - 20, 6, 40, 12);
            window.SetLine("@made by Inko (inkooognito@gmail.com) 2016", window.CenterX - 20, window.sizeY - 5);

            window.SetLine("[1] Host Server", window.CenterX - 10, 24);
            window.SetLine("[2] Join Server", window.CenterX - 10, 26);
            window.SetLine("[3] Debug Mode", window.CenterX - 10, 28);


            while (true)
            {
                input.CatchInput();
                if (input.GetKey(KeyCode.D1))
                {
                    selection = 0;
                    break;
                }
                else if (input.GetKey(KeyCode.D2))
                {
                    selection = 1;
                    break;
                }
                else if (input.GetKey(KeyCode.D3))
                {
                    selection = 2;
                    break;
                }
            }

            window.Wait(.1f);
            window.Clear();

            window.SetPosition(0, 0);
            return selection;
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
