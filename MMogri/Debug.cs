using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMogri.Debugging
{
    static class Debug
    {
        public static void Log (object o)
        {
            string s = o.ToString();
            if (o == null)
                s = "NULL";
            Console.WriteLine(s);
            System.Diagnostics.Debug.Print(s);
        }
    }
}
