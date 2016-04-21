using MMogri.Scripting;

namespace MMogri.Gameplay
{
    class ItemSlot : ScriptableDataContainer
    {
        public byte itemId;
        public byte count;

        public ItemSlot() { }

        public ItemSlot(byte itemId, byte count)
        {
            this.itemId = itemId;
            this.count = count;
        }
    }
}
