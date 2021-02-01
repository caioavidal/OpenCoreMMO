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
        ushort Id { get; init; }
        string Name { get; set; }
        IDictionary<ushort, IGuildLevel> GuildLevels { get; set; }
        IChatChannel Channel { get; set; }

        IGuildLevel GetMemberLevel(IPlayer player);
        bool HasMember(IPlayer player);
    }
    public interface IGuildLevel
    {
        GuildRank Level { get; }
    }
}
