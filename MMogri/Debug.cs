using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMogri.Debugging
{
    static class Debug
    {
        public static void Log(object o)
        {
            string s;
            if (o == null)
                s = "NULL";
            else
                s = o.ToString();
            //Console.WriteLine(s);
            System.Diagnostics.Debug.Print(s);
        }
    }
}
