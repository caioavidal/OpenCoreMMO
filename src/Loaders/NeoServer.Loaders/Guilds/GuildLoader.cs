using NeoServer.Data.Interfaces;
using NeoServer.Data.Model;
using NeoServer.Game.Chats;
using NeoServer.Game.Common.Creatures.Guilds;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Creatures.Guilds;
using NeoServer.Game.DataStore;
using NeoServer.Loaders.Attributes;
using NeoServer.Loaders.Interfaces;
using NeoServer.Server.Model.Players;
using Serilog.Core;
using System.Collections.Generic;

namespace NeoServer.Loaders.Guilds
{
    public class GuildLoader: ICustomLoader
    {
        private readonly Logger logger;
        private readonly ChatChannelFactory chatChannelFactory;

        public GuildLoader(Logger logger, ChatChannelFactory chatChannelFactory)
        {
            this.logger = logger;
            this.chatChannelFactory = chatChannelFactory;
        }

        public void Load(GuildModel guildModel)
        {
            if (guildModel is not GuildModel) return;

            var guild = GuildStore.Data.Get((ushort)guildModel.Id);

            var shouldAddToStore = false;
            if (guild is null)
            {
                shouldAddToStore = true;
                guild = new Guild
                {
                    Id = (ushort)guildModel.Id,
                    Channel = chatChannelFactory.CreateGuildChannel($"{guildModel.Name}'s Channel", (ushort)guildModel.Id)
                };
            }

            guild.Name = guildModel.Name;
            guild.GuildLevels?.Clear();

            if ((guildModel.Ranks?.Count ?? 0) > 0)
                guild.GuildLevels = new Dictionary<ushort, IGuildLevel>();

            foreach (var member in guildModel.Members)
            {
                if (member.Rank is null) continue;
                guild.GuildLevels.Add((ushort)member.Rank.Id, new GuildLevel((GuildRank)(member.Rank?.Level ?? (int)GuildRank.Member), member.Rank?.Name));
            }

            if (shouldAddToStore)
            {
                GuildStore.Data.Add(guild.Id, guild);
                return;
            }

            logger.Debug("Guild {guild} loaded", guildModel.Name);
        }
    }
}
