using System;
using System.Threading;

namespace MMogri.Input
{
    class InputHandler
    {
        bool catchKey;
        bool catchLine;

        ConsoleKeyInfo? keyInf;
        string input;
        Action<string> catchCallback;

        public InputHandler()
        {
            var thread = new Thread(() =>
            {
                while (true)
                {
                    if (catchKey)
                    {
                        keyInf = System.Console.ReadKey(true);
                        catchKey = false;
                    }
                    if (catchLine)
                    {
                        input = System.Console.ReadLine();

                        if (catchCallback != null) catchCallback(input);
                        catchLine = false;
                        catchCallback = null;
                    }
                }
            });

            thread.IsBackground = true;
            thread.Start();
        }

        public void CatchInput()
        {
            catchKey = true;
        }

        public void CatchLine(Action<string> a)
        {
            catchLine = true;
            catchCallback = a;
        }

        public bool GetKey(KeyCode key, KeyCode altKey = KeyCode.NoName)
        {
            if (keyInf == null) return false;

            if (((ConsoleKeyInfo)keyInf).Key == key.ToConsoleKey() || ((ConsoleKeyInfo)keyInf).Key == altKey.ToConsoleKey())
            {
                keyInf = null;
                return true;
            }
            return false;
        }
    }
}
