using NeoServer.Game.Chats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Scripts.Chat
{
    public class LootChannel : PersonalChatChannel
    {
        public LootChannel(ushort id, string name) : base(id, name)
        {
        }
    }
}
