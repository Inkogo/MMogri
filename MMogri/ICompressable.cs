namespace MMogri
{
    public interface ICompressable
    {
        byte[] ToBytes();

        void FromBytes(byte[] b);
    }
}
