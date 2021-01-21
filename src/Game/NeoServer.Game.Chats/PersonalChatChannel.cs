using NeoServer.Game.Common;
using NeoServer.Game.Common.Talks;
using NeoServer.Game.Contracts.Chats;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeoServer.Game.Chats
{
    public abstract class PersonalChatChannel : ChatChannel, IChatChannel
    {
        public PersonalChatChannel(ushort id, string name) : base(id, name)
        {
        }
        public abstract new string Name { get; }
        public override bool AddUser(IPlayer player)
        {
            if (users.Count == 1) return false;
            return base.AddUser(player);
        }
    }
}
