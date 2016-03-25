using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMogri
{
    class UserPreferences
    {
        static UserPreferences _instance;

        public static UserPreferences Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new UserPreferences();
                return _instance;
            }
        }

        readonly public int windowSizeX = 20;
        readonly public int windowSizeY = 20;
    }
}
