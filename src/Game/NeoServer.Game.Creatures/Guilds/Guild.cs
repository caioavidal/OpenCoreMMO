using NeoServer.Game.Contracts.Chats;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Creatures.Guilds
{
    public class Guild : IGuild
    {
        public ushort Id { get; set; }
        public string Name { get; set; }
        public HashSet<IGuildMember> GuildMembers { get; set; }
        public IChatChannel Channel { get; set; }
        public bool HasMember(IPlayer player) => GuildMembers?.Contains(new GuildMember(player.Id)) ?? false;
        public bool HasMember(uint playerId) => GuildMembers?.Contains(new GuildMember(playerId)) ?? false;

    }

    public struct GuildMember : IGuildMember, IEquatable<GuildMember>
    {
        public uint PlayerId { get; private set; }
        public GuildRank Level { get; private set; }
        private string levelName;

        public GuildMember(uint playerId, GuildRank level = GuildRank.Member, string levelName = null) : this()
        {
            PlayerId = playerId;
            Level = level;
            LevelName = levelName;
        }

        public string LevelName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(levelName)) return levelName;
                return Level switch
                {
                    GuildRank.Leader => "Leader",
                    GuildRank.ViceLeader => "Vice-Leader",
                    _ => "Member"
                };
            }
            private set
            {
                if (string.IsNullOrWhiteSpace(value)) return;
                levelName = value;
            }
        }

        public bool Equals(GuildMember other)
        {
            return other.PlayerId == PlayerId;
        }
        public override bool Equals(object obj) => obj is GuildMember member && member.PlayerId == member.PlayerId;

        public override int GetHashCode() => HashCode.Combine(PlayerId);
    }

    public enum GuildRank
    {
        Leader = 1,
        ViceLeader = 2,
        Member = 3
    }
}
