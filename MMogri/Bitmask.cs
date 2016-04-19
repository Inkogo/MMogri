namespace MMogri.Utils
{
    [System.Serializable]
    public class Bitmask
    {
        //forms a bitMask from 0 - 255
        ulong mask;

        public Bitmask(ulong mask)
        {
            this.mask = mask;
        }

        public Bitmask(params bool[] bits)
        {
            this.mask = 0ul;
            for (int i = 0; i < bits.Length; i++)
                SetBit(i, bits[i]);
        }

        public bool CheckBit(int bitToReturn)
        {
            ulong m = 1ul << bitToReturn;
            return (mask & m) == m;
        }

        public void SetBit(int n, bool b)
        {
            if (b)
                mask = (ulong)(mask | (1ul << n));
            else
                mask = (ulong)(mask & ~(1ul << n));
        }

        public static implicit operator ulong (Bitmask m)
        {
            return m.mask;
        }

        public static implicit operator Bitmask(ulong l)
        {
            return new Bitmask(l);
        }
    }
}