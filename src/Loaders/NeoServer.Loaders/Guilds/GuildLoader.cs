﻿using System.Collections.Generic;
using NeoServer.Data.Model;
using NeoServer.Game.Chats;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Creatures.Guilds;
using NeoServer.Game.Creatures.Guilds;
using NeoServer.Game.DataStore;
using NeoServer.Loaders.Interfaces;
using Serilog.Core;

namespace NeoServer.Loaders.Guilds
{
    public class GuildLoader : ICustomLoader
    {
        private readonly ChatChannelFactory chatChannelFactory;
        private readonly Logger logger;

        public GuildLoader(Logger logger, ChatChannelFactory chatChannelFactory)
        {
            this.logger = logger;
            this.chatChannelFactory = chatChannelFactory;
        }

        public void Load(GuildModel guildModel)
        {
            if (guildModel is not GuildModel) return;

            var guild = GuildStore.Data.Get((ushort) guildModel.Id);

            var shouldAddToStore = false;
            if (guild is null)
            {
                shouldAddToStore = true;
                guild = new Guild
                {
                    Id = (ushort) guildModel.Id,
                    Channel = chatChannelFactory.CreateGuildChannel($"{guildModel.Name}'s Channel",
                        (ushort) guildModel.Id)
                };
            }

            guild.Name = guildModel.Name;
            guild.GuildLevels?.Clear();

            if ((guildModel.Ranks?.Count ?? 0) > 0)
                guild.GuildLevels = new Dictionary<ushort, IGuildLevel>();

            foreach (var member in guildModel.Members)
            {
                if (member.Rank is null) continue;
                guild.GuildLevels.Add((ushort) member.Rank.Id,
                    new GuildLevel((GuildRank) (member.Rank?.Level ?? (int) GuildRank.Member), member.Rank?.Name));
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