using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Chats
{
    public class NpcChannel : ChatChannel
    {
        public NpcChannel(ushort id, string name) : base(id, name)
        {
        }
    }
}
