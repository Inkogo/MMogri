using System.Collections;
using System;

namespace MMogri.Network
{
    public class RPC : Attribute
    {
        public string tag;

        public RPC(string tag)
        {
            this.tag = tag;
        }
    }
}