using System.Collections.Generic;
using NeoServer.Data.Entities;
using NeoServer.Game.Chats;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Common.Creatures.Guilds;
using NeoServer.Game.Creatures.Guild;
using NeoServer.Loaders.Interfaces;
using Serilog;

namespace NeoServer.Loaders.Guilds;

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

    public void Load(GuildEntity guildEntity)
    {
        if (guildEntity is null) return;

        var guild = _guildStore.Get((ushort)guildEntity.Id);

        var shouldAddToStore = false;
        if (guild is null)
        {
            shouldAddToStore = true;
            guild = new Guild
            {
                Id = (ushort)guildEntity.Id,
                Channel = _chatChannelFactory.CreateGuildChannel($"{guildEntity.Name}'s Channel",
                    (ushort)guildEntity.Id)
            };
        }

        guild.Name = guildEntity.Name;
        guild.GuildLevels?.Clear();

        if ((guildEntity.Ranks?.Count ?? 0) > 0)
            guild.GuildLevels = new Dictionary<ushort, IGuildLevel>();

        foreach (var member in guildEntity.Members)
        {
            if (member.Rank is null) continue;
            guild.GuildLevels?.Add((ushort)member.Rank.Id,
                new GuildLevel((GuildRank)(member.Rank?.Level ?? (int)GuildRank.Member), member.Rank?.Name));
        }

        if (shouldAddToStore)
        {
            _guildStore.Add(guild.Id, guild);
            return;
        }

        _logger.Debug("Guild {guild} loaded", guildEntity.Name);
    }
}