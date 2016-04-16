using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMogri.Security
{
    class Account
    {
        public enum AccountStatus
        {
            none, mod, admin, banned
        }

        public Guid Id;

        public AccountStatus status;
        public string email;
        public Guid password;
        public Guid sessionId;
        public DateTime lastLogin;
        public DateTime createdAt;

        public Account () : this("", Guid.Empty, 0)
        { }

        public Account(string email, Guid password, AccountStatus status)
        {
            this.Id = Guid.NewGuid();
            this.email = email;
            this.password = password;
            this.status = status;

            Id = Guid.NewGuid();
            lastLogin = createdAt = DateTime.Now;
        }

        public bool ComparePassword(Guid g)
        {
            return password.Equals(g);
        }

        public AccountStatus GetStatus
        {
            get
            {
                return status;
            }
        }
    }
}
