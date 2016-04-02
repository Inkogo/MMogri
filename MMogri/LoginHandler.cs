using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using MMogri.Gameplay;
using MMogri.Utils;

namespace MMogri.Security
{
    class LoginHandler
    {
        LocalDb<Account> accountLoader;
        LocalDb<Player> playerLoader;

        public LoginHandler(string path)
        {
            accountLoader = new LocalDb<Account>(Path.Combine(path, "playerData.db"), new SQLiteWrapper("Accounts", new SQLiteWrapper.SqlItem[] {
                new SQLiteWrapper.SqlItem ("Id", SQLiteWrapper.SQLiteType.TEXT, typeof(Account).GetField("Id"), true, new GuidToStringConverter() ),
                new SQLiteWrapper.SqlItem ("Status", SQLiteWrapper.SQLiteType.TEXT, typeof(Account).GetField("status"), false, new EnumToStringConverter<Account.AccountStatus>() ),
                new SQLiteWrapper.SqlItem ("Email", SQLiteWrapper.SQLiteType.TEXT, typeof(Account).GetField("email") ),
                new SQLiteWrapper.SqlItem ("Password", SQLiteWrapper.SQLiteType.TEXT, typeof(Account).GetField("password"), false, new GuidToStringConverter() ),
                new SQLiteWrapper.SqlItem ("LastLogin", SQLiteWrapper.SQLiteType.TEXT, typeof(Account).GetField("lastLogin"), false, new DateTimeToStringConverter() ),
                new SQLiteWrapper.SqlItem ("CreatedAt", SQLiteWrapper.SQLiteType.TEXT, typeof(Account).GetField("createdAt"), false, new DateTimeToStringConverter() ),
            }));

            playerLoader = new LocalDb<Player>(Path.Combine(path, "playerData.db"), new SQLiteWrapper("Players", new SQLiteWrapper.SqlItem[] {
                new SQLiteWrapper.SqlItem("Id", SQLiteWrapper.SQLiteType.TEXT, typeof(Player).GetField("Id"), true, new GuidToStringConverter() ),
                new SQLiteWrapper.SqlItem("AccountId", SQLiteWrapper.SQLiteType.TEXT, typeof(Player).GetField("accountId"), false, new GuidToStringConverter() ),
                new SQLiteWrapper.SqlItem("MapId", SQLiteWrapper.SQLiteType.TEXT, typeof(Player).GetField("mapId"), false, new GuidToStringConverter() ),
                new SQLiteWrapper.SqlItem("Name", SQLiteWrapper.SQLiteType.TEXT, typeof(Player).GetField("name") ),
                new SQLiteWrapper.SqlItem("PosX", SQLiteWrapper.SQLiteType.INTEGER, typeof(Player).GetField("x") ),
                new SQLiteWrapper.SqlItem("PosY", SQLiteWrapper.SQLiteType.INTEGER, typeof(Player).GetField("y") ),
                new SQLiteWrapper.SqlItem("Lvl", SQLiteWrapper.SQLiteType.INTEGER, typeof(Player).GetField("lvl") ),
                new SQLiteWrapper.SqlItem("Exp", SQLiteWrapper.SQLiteType.INTEGER, typeof(Player).GetField("exp") ),
            }));
        }

        public Account GetAccount(string name, Guid password)
        {
            Account a = accountLoader.GetItems("Where Email = '" + name + "'").FirstOrDefault();
            if (a != null && a.ComparePassword(password)) return a;
            return null;
        }

        public void CreateAccount(string name, Guid password, Account.AccountStatus status = Account.AccountStatus.none)
        {
            accountLoader.AddItem(new Account(name, password, status));
        }

        public Player[] GetPlayersOfAccount(Guid AccountId)
        {
            return playerLoader.GetItems("Where AccountId ='" + AccountId + "'").ToArray();
        }

        public void CreatePlayer(string name, Guid account, Guid map, int x, int y)
        {
            playerLoader.AddItem(new Player()
            {
                name = name,
                accountId = account,
                mapId = map,
                x = x,
                y = y,
                lvl = 1,
                exp = 0,
            });
        }
    }
}
