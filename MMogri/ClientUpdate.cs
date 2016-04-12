using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMogri
{
    abstract public class ClientUpdate
    {
        // mapUpdate inherits from this, so does entityUpdate, playerUpdate, worldUpdate
        // also have accessor to deserialize bitStream from enum short to T via dictionary<short, type>
    }
}
