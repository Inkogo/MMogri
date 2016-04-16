using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace MMogri.Input
{
    class InputHandler
    {
        bool catchKey;
        ConsoleKeyInfo? keyInf;

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
                }
            });

            thread.IsBackground = true;
            thread.Start();
        }

        public void CatchInput()
        {
            catchKey = true;
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
