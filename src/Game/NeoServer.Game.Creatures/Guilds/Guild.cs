using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Creatures.Guilds
{
    public class Guild: IGuild
    {
        public ushort Id { get; set; }
        public string Name { get; set; }
        public HashSet<IGuildMember> GuildMembers { get; set; }
    }

    public struct GuildMember : IGuildMember, IEquatable<GuildMember>
    {
        public uint PlayerId { get; set; }
        public GuildRank Level { get; set; }
        public string LevelName { get; set; }

        public bool Equals(GuildMember other)
        {
            return other.PlayerId == PlayerId;
        }
        public override bool Equals(object obj) => obj is GuildMember member && member.PlayerId == member.PlayerId;

        public override int GetHashCode() => HashCode.Combine(PlayerId);
    }

    public enum GuildRank
    {
        Leader,
        ViceLeader,
        Member = default
    }
}
