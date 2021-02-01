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
    //public class GuildLoader : ICustomLoader
    //{
    //    private readonly IGuildRepository guildRepository;
    //    private readonly Logger logger;
    //    private readonly ChatChannelFactory chatChannelFactory;

    //    public GuildLoader(IGuildRepository guildRepository, Logger logger, ChatChannelFactory chatChannelFactory)
    //    {
    //        this.guildRepository = guildRepository;
    //        this.logger = logger;
    //        this.chatChannelFactory = chatChannelFactory;
    //    }

    //    public async void Load()
    //    {
    //        var guilds = await guildRepository.GetAll();

    //        foreach (var guildModel in guilds)
    //        {
    //            var guild = new Guild
    //            {
    //                Id = (ushort)guildModel.Id,
    //                Name = guildModel.Name,
    //                Channel = chatChannelFactory.CreateGuildChannel($"{guildModel.Name}'s Channel", (ushort)guildModel.Id)
    //            };
    //            guild.GuildMembers = new HashSet<Game.Contracts.Creatures.IGuildMember>();

    //            foreach (var member in guildModel.Members)
    //            {
    //                guild.GuildMembers.Add(new GuildMember((uint)member.PlayerId, (GuildRank)(member.Rank?.Level ?? (int)GuildRank.Member), member.Rank?.Name));
    //            }

    //            GuildStore.Data.Add(guild.Id, guild);

    //            logger.Debug("Guilds loaded!");
    //        }
    //    }
    //}
}
