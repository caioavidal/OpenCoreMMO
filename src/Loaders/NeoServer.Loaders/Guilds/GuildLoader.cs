using System.Collections.Generic;
using NeoServer.Data.Model;
using NeoServer.Game.Chats;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Common.Creatures.Guilds;
using NeoServer.Game.Creatures.Guilds;
using NeoServer.Loaders.Interfaces;
using Serilog;

namespace NeoServer.Loaders.Guilds
{
    public class GuildLoader : ICustomLoader
    {
        private readonly ChatChannelFactory _chatChannelFactory;
        private readonly IGuildStore _guildStore;
        private readonly ILogger _logger;

        public GuildLoader(ILogger logger, ChatChannelFactory chatChannelFactory, IGuildStore guildStore)
        {
            _logger = logger;
            _chatChannelFactory = chatChannelFactory;
            _guildStore = guildStore;
        }

        public void Load(GuildModel guildModel)
        {
            if (guildModel is not { }) return;

            var guild = _guildStore.Get((ushort) guildModel.Id);

            var shouldAddToStore = false;
            if (guild is null)
            {
                shouldAddToStore = true;
                guild = new Guild
                {
                    Id = (ushort) guildModel.Id,
                    Channel = _chatChannelFactory.CreateGuildChannel($"{guildModel.Name}'s Channel",
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
                guild.GuildLevels?.Add((ushort) member.Rank.Id,
                    new GuildLevel((GuildRank) (member.Rank?.Level ?? (int) GuildRank.Member), member.Rank?.Name));
            }

            if (shouldAddToStore)
            {
                _guildStore.Add(guild.Id, guild);
                return;
            }

            _logger.Debug("Guild {guild} loaded", guildModel.Name);
        }
    }
}