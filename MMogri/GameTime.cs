using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMogri
{
    class GameTime
    {
        public int time;

        public GameTime(int hour, int day, int month, int year)
        {
            //time = ...;
        }

        public static GameTime operator +(GameTime t, int i)
        {
            t.time += i;
            return t;
        }

        public int Hour { get { return 0; } }

        public int Day { get { return 0; } }

        public int Month { get { return 0; } }

        public int Year { get { return 0; } }
    }
}
