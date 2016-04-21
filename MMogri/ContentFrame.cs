using MMogri.Input;
using MMogri.Renderer;
using System;

namespace MMogri
{
    abstract class ContentFrame
    {
        const char borderHorizontal = '═';
        const char borderVertical = '║';
        const char borderCornerTopLeft = '╔';
        const char borderCornerTopRight = '╗';
        const char borderCornerBotLeft = '╚';
        const char borderCornerBotRight = '╝';

        public int posX, posY, width, height;

        protected GameWindow window;
        protected InputHandler input;

        public ContentFrame(GameWindow w, InputHandler i)
        {
            window = w;
            input = i;
        }

        public void SetFrame(int px, int py, int w, int h)
        {
            posX = px;
            posY = py;
            width = w;
            height = h;

            DrawFrame();
        }

        public void ClearFrame()
        {
            window.Clear(posX + 1, posY + 1, width - 2, height - 2);
        }

        public void DrawFrame(int posX, int posY, int width, int height)
        {
            SetFrame(posX, posY, width, height);
            DrawFrame();
        }

        public void DrawFrame()
        {
            for (int y = 0; y < height; y++)
            {
                Console.SetCursorPosition(posX, posY + y);

                for (int x = 0; x < width; x++)
                {
                    if (x == 0 && y == 0)
                        window.SetNext(borderCornerTopLeft);
                    else if (x == width - 1 && y == 0)
                        window.SetNext(borderCornerTopRight);
                    else if (x == width - 1 && y == 0)
                        window.SetNext(borderCornerTopRight);
                    else if (x == 0 && y == height - 1)
                        window.SetNext(borderCornerBotLeft);
                    else if (x == width - 1 && y == height - 1)
                        window.SetNext(borderCornerBotRight);

                    else if (x == 0 || x == width - 1)
                        window.SetNext(borderVertical);
                    else if (y == 0 || y == height - 1)
                        window.SetNext(borderHorizontal);
                    else
                        window.SetNext(' ');

                }
                window.SetNext(Environment.NewLine);
            }
        }

        abstract public void Start();
    }
}
