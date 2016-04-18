using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace MMogri.Renderer
{
    class GameWindow
    {
        public int sizeX;
        public int sizeY;

        public GameWindow(int x, int y)
        {
            sizeX = x;
            sizeY = y;
            Console.BufferWidth = Console.WindowWidth = x;
            Console.BufferHeight = Console.WindowHeight = y;

            Console.CursorVisible = false;
        }

        public void SetTile(char t, Color c, int x, int y)
        {
            Console.SetCursorPosition(x, y);
            Console.ForegroundColor = c.ToConsoleColor();
            Console.Write(t);
            Console.ResetColor();
        }

        public void SetPosition(int x, int y)
        {
            Console.SetCursorPosition(x, y);
        }

        public void SetChar(char c, int x, int y)
        {
            Console.SetCursorPosition(x, y);
            Console.Write(c);
        }

        public void SetLine(string s, int x, int y)
        {
            Console.SetCursorPosition(x, y);
            Console.WriteLine(s);
        }

        public void SetColor (Color c)
        {
            Console.ForegroundColor = c.ToConsoleColor();
        }

        public void SetNext(char c)
        {
            Console.Write(c);
        }

        public void SetNext(string s)
        {
            Console.Write(s);
        }

        public void SetRect(string t, int x, int y, int sX, int sY)
        {
            int n = 0;
            using (System.IO.StringReader reader = new System.IO.StringReader(t))
            {
                while (reader.Peek() >= 0)
                {
                    Console.SetCursorPosition(x, y + n);
                    Console.WriteLine(reader.ReadLine());
                    n++;
                }
            }
        }

        public void SetEditMode (bool b)
        {
            Console.CursorVisible = b;
        }

        public void Clear ()
        {
            Console.Clear();
        }

        public void Clear (int x, int y, int sx, int sy, char c = ' ')
        {
            for(int i= y;i< y + sy;i++)
            {
                Console.SetCursorPosition(x, i);
                Console.WriteLine(new string(c, sx));
            }
        }

        public void Wait (float f)
        {
            Thread.Sleep((int)(f * 1000));
        }

        public int CenterX
        {
            get
            {
                return (int)Math.Round(sizeX * .5f);
            }
        }

        public int CenterY
        {
            get
            {
                return (int)Math.Round(sizeY * .5f);
            }
        }
    }
}
