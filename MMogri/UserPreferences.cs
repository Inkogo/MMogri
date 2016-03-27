using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMogri.Utils;
using System.IO;

namespace MMogri
{
    public class UserPreferences
    {
        static UserPreferences _instance;

        public static UserPreferences Instance
        {
            get
            {
                if (_instance == null)
                    if (!LoadUserPreferences())
                    {
                        _instance = new UserPreferences();
                        SaveUserPreferences();
                    }
                return _instance;
            }
        }

        static bool LoadUserPreferences()
        {
            if (!File.Exists(PrefPath)) return false;

            _instance = FileUtils.LoadFromXml<UserPreferences>(PrefPath);
            return true;
        }

        static void SaveUserPreferences()
        {
            FileUtils.SaveToXml<UserPreferences>(_instance, PrefPath);
        }

        public int windowSizeX = 70;
        public int windowSizeY = 46;

        static string PrefPath
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UserPrefs.xml");
            }
        }
    }
}
