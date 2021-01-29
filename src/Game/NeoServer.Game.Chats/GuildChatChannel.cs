using NeoServer.Game.Common;
using NeoServer.Game.Common.Talks;
using NeoServer.Game.Contracts.Chats;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.DataStore;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeoServer.Game.Chats
{
    public  class GuildChatChannel : ChatChannel, IChatChannel
    {
        public GuildChatChannel(ushort id, string name, ushort guildId) : base(id, name)
        {
            GuildId = guildId;
        }
        public override bool Opened { get => true; init => base.Opened = value; }
        public ushort GuildId { get; }
        public IGuild Guild => GuildStore.Data.Get(GuildId);
        public override bool AddUser(IPlayer player)
        {
            if (player.GuildId == 0) return false;
            if (Guild is null) return false;

            if (!Guild.HasMember(player)) return false;

            return base.AddUser(player);
        }
    }
}
