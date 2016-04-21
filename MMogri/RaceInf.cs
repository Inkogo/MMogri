using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMogri.Gameplay
{
    [System.Serializable]
    public class RaceInf
    {
        public string name;
        public int maxAge;

        public RaceInf () { }

        public RaceInf (string n, int age) {
            name = n;
            maxAge = age;
        }
    }
}
