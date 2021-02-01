using NeoServer.Game.Common.Creatures.Guilds;
using NeoServer.Game.Contracts.Chats;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Contracts.Creatures
{
    public interface IGuild
    {
        ushort Id { get; set; }
        string Name { get; set; }
        HashSet<IGuildMember> GuildMembers { get; set; }
        IChatChannel Channel { get; set; }

        IGuildMember GetMember(uint playerId);
        bool HasMember(IPlayer player);
        bool HasMember(uint playerId);
    }
    public interface IGuildMember
    {
        GuildRank Level { get; }
    }
}
