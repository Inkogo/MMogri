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
        ConsoleKeyInfo? keyInf;

        public void CatchInput()
        {
            var thread = new Thread(() =>
            {
                bool keyHit = false;
                while (!keyHit)
                {
                    keyInf = System.Console.ReadKey(true);

                    keyHit = keyInf != null;
                }
            });

            thread.IsBackground = true;
            thread.Start();
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
