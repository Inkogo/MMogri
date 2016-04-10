using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMogri
{
    public interface ICompressable
    {
        byte[] ToBytes();

        void FromBytes(byte[] b);
    }
}
