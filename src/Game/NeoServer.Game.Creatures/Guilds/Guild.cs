using NeoServer.Game.Common.Creatures.Guilds;
using NeoServer.Game.Contracts.Chats;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Creatures;
using System;
using System.Collections.Generic;

namespace NeoServer.Game.Creatures.Guilds
{
    public class Guild : IGuild
    {
        public ushort Id { get; init; }
        public string Name { get; set; }
        public IDictionary<ushort, IGuildLevel> GuildLevels { get; set; }
        public IChatChannel Channel { get; set; }
        public bool HasMember(IPlayer player) => player.GuildId == Id;
        public IGuildLevel GetMemberLevel(IPlayer player) => GuildLevels is null ? null : GuildLevels.TryGetValue(player.Level, out var level) ? level : null;
    }

    public class GuildLevel : IGuildLevel, IEquatable<GuildLevel>
    {
        public ushort Id { get; init; }
        public GuildRank Level { get; private set; }
        private string levelName;

        public GuildLevel(GuildRank level = GuildRank.Member, string levelName = null)
        {
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

        public bool Equals(GuildLevel other)
        {
            return other.Id == Id;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
    }

}
