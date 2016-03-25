using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMogri.Renderer
{
    public enum Color
    {
        Black = 0,
        DarkBlue = 1,
        DarkGreen = 2,
        DarkCyan = 3,
        DarkRed = 4,
        DarkMagenta = 5,
        DarkYellow = 6,
        Gray = 7,
        DarkGray = 8,
        Blue = 9,
        Green = 10,
        Cyan = 11,
        Red = 12,
        Magenta = 13,
        Yellow = 14,
        White = 15
    }


    public static class ColorExtensions
    {
        public static ConsoleColor ToConsoleColor(this Color value)
        {
            try
            {
                ConsoleColor c = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), value.ToString());
                return c;
            }
            catch
            {
                return ConsoleColor.Black;
            }
        }
    }
}
