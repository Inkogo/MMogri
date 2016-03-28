using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMogri.Gameplay;

namespace MMogri.Security
{
    class LoginHandler
    {
        DBLoader<Account> accountLoader = new DBLoader<Account>();
        DBLoader<Player> playerLoader = new DBLoader<Player>();

        static LoginHandler _instance;
        public static LoginHandler Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new LoginHandler();
                return _instance;
            }
        }

        public Account GetAccount(string name, Guid password)
        {
            Account a = accountLoader.GetItems("name =" + name).First();
            if (a.ComparePassword(password)) return a;
            return null;
        }

        public Player[] GetPlayersOfAccount(Account a)
        {
            return playerLoader.GetItems("accountId =" + a.Id).ToArray();
        }
    }
}
