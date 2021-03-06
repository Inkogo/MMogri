﻿using MMogri.Gameplay;
using MMogri.Utils;
using System;
using System.IO;
using System.Linq;

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
                new SQLiteWrapper.SqlItem ("SessionToken", SQLiteWrapper.SQLiteType.TEXT, typeof(Account).GetField("sessionToken") ),
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
                new SQLiteWrapper.SqlItem("PlayerState", SQLiteWrapper.SQLiteType.TEXT, typeof(Player).GetField("playerState") ),
                new SQLiteWrapper.SqlItem("Stats", SQLiteWrapper.SQLiteType.BLOB, typeof(Player).GetField("stats"), false, new ICompressableToBytesConverter<CharacterStats>() ),
            }));
        }

        public Account FindAccount(Guid id)
        {
            return accountLoader.GetItems("Where Id = '" + id + "'").FirstOrDefault();
        }

        public Account FindAccount(string email)
        {
            return accountLoader.GetItems("Where Email = '" + email + "'").FirstOrDefault();
        }

        public Player FindPlayer(Guid account, string name)
        {
            return playerLoader.GetItems("Where Name = '" + name + "' AND AccountId = '" + account + "'").FirstOrDefault();
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

        public Player CreatePlayer(string name, Guid account, Guid map, int x, int y)
        {
            Player p = new Player()     //change this to a constructor!
            {
                name = name,
                accountId = account,
                mapId = map,
                x = x,
                y = y,
                lvl = 1,
                exp = 0,
                playerState = "",
                stats = new CharacterStats(),
            };

            playerLoader.AddItem(p);
            return p;
        }

        public void UpdatePlayer(Player p)
        {
            playerLoader.UpdateItem(p, "Where Id = '" + p.Id + "'");
        }

        public string GenSessionToken(Guid AccountId)
        {
            byte[] time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
            byte[] key = Guid.NewGuid().ToByteArray();
            byte[] id = BitConverter.GetBytes(AccountId.GetHashCode());
            string token = Convert.ToBase64String(key.Concat(time).Concat(id).ToArray());

            Account a = accountLoader.GetItems("Where Id = '" + AccountId + "'").FirstOrDefault();
            a.sessionToken = token;
            a.lastLogin = System.DateTime.Now;

            accountLoader.UpdateItem(a, "Where Id = '" + a.Id + "'");
            return token;
        }

        public bool ValidateSessionId(Guid AccountId, string SessionId)
        {
            Account a = accountLoader.GetItems("Where Id = '" + AccountId + "'").FirstOrDefault();
            return a.sessionToken.Equals(SessionId) && (a.lastLogin - DateTime.Now).Seconds < 150;     //150 is 1.5 mins timeout
        }

        public void ResetPassword(string targetEmail, string ownerEmail, string ownerPassword)
        {
            Account a = accountLoader.GetItems("Where Email = '" + targetEmail + "'").FirstOrDefault();
            string newPassword = "XXXXXXXXX".Random();
            a.password = newPassword.ToGuid();
            accountLoader.UpdateItem(a, "Where Id = '" + a.Id + "'");

            EMailHandler.Send(ownerEmail, ownerPassword, targetEmail, "Your Password has been reset!",
@"This is an automated Email to reset your password in the online game MMogri. 
If you did not reset your account password please ignore this email!

The new Password: " + newPassword);
        }

        public void ChangePassword(string name, Guid oldPassword, Guid newPassword)
        {
            Account a = accountLoader.GetItems("Where Email = '" + name + "'").FirstOrDefault();
            if (a.ComparePassword(oldPassword))
            {
                a.password = newPassword;
                accountLoader.UpdateItem(a, "Where Id = '" + a.Id + "'");
            }
        }
    }
}
