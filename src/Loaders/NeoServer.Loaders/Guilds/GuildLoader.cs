using NeoServer.Data.Interfaces;
using NeoServer.Game.Creatures.Guilds;
using NeoServer.Game.DataStore;
using NeoServer.Loaders.Interfaces;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Loaders.Guilds
{
    public class GuildLoader : ICustomLoader
    {
        private readonly IGuildRepository guildRepository;
        private readonly Logger logger;

        public GuildLoader(IGuildRepository guildRepository, Logger logger)
        {
            this.guildRepository = guildRepository;
            this.logger = logger;
        }

        public async void Load()
        {
            var guilds = await guildRepository.GetAll();

            foreach (var guildModel in guilds)
            {
                var guild = new Guild
                {
                    Id = (ushort)guildModel.Id,
                    Name = guildModel.Name
                };
                guild.GuildMembers = new HashSet<Game.Contracts.Creatures.IGuildMember>();

                foreach (var member in guildModel.Members)
                {
                    guild.GuildMembers.Add(new GuildMember()
                    {
                        PlayerId = (uint)member.PlayerId,
                        Level = (GuildRank)member.Rank.Level,
                        LevelName = member.Rank.Name
                    });
                }

                GuildStore.Data.Add(guild.Id, guild);

                logger.Debug("Guilds loaded!");
            }
        }
    }
}
