using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMogri.Gameplay
{
    [System.Serializable]
    public class Player : Entity
    {
        public Guid Id;
        public Guid accountId;

        public Guid mapId;

        public int lvl;
        public int exp;

        public CharacterStats stats;

        public Player()
        {
            Id = Guid.NewGuid();
        }
    }
}
