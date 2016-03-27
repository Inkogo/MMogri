using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMogri.Gameplay
{
    [System.Serializable]
    class Player : Entity
    {
        public Guid Id;
        public Guid accountId;

        public Guid mapId;

        public int gender;
        public int characterClass;

        public int lvl;
        public int exp;

        public CharacterStats stats;
        public Inventory inventory;
        public Equipment equipment;

    }
}
