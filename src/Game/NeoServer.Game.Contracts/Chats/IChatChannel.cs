using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Contracts.Chats
{
    public interface IChatChannel
    {
        ushort Id { get; }
        string Name { get; }
    }
}
