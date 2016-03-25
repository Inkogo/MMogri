using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMogri.Input
{
    class InputHandler
    {
        ConsoleKeyInfo keyInf;

        public void CatchInput ()
        {
            keyInf = Console.ReadKey(true);
        }

        public bool GetKey(KeyCode key, KeyCode altKey = KeyCode.NoName)
        {
            if (keyInf == null) return false;

            if (keyInf.Key == key.ToConsoleKey() || keyInf.Key == altKey.ToConsoleKey())
            {
                return true;
            }
            return false;
        }

    }
}
