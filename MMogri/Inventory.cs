using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMogri.Gameplay
{
    class Inventory
    {
        public struct ItemSlot {
            public byte itemId;
            public byte numb;
            public byte addData;
        }

        public List<ItemSlot> slots;
    }
}
