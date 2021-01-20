using NeoServer.Game.Chats;
using NeoServer.Game.Common;
using NeoServer.Game.DataStore;
using NeoServer.Loaders.Chats;
using NeoServer.Loaders.Interfaces;
using NeoServer.Server.Standalone;
using Newtonsoft.Json;
using Serilog.Core;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NeoServer.Scripts.Chats
{
    public class ChannelLoader : ICustomLoader
    {
        private readonly ServerConfiguration serverConfiguration;
        private readonly Logger logger;
        public ChannelLoader(ServerConfiguration serverConfiguration, Logger logger)
        {
            this.serverConfiguration = serverConfiguration;
            this.logger = logger;
        }
        public void Load()
        {
            var jsonString = File.ReadAllText(Path.Combine(serverConfiguration.Data, "channels.json"));

            var channels = JsonConvert.DeserializeObject<List<ChannelModel>>(jsonString);

            foreach (var channel in channels)
            {
                var id = RandomIdGenerator.Generate(ushort.MaxValue);

                ChatChannelStore.Data.Add(id, new ChatChannel(id, channel.Name)
                {
                    ChatColor = (TextColor?)channel.Color?.Default ?? TextColor.Black,
                    ChatColorByVocation = channel.Color?.ByVocation?.ToDictionary(x => (byte)x.Key, x => (TextColor)x.Value) ?? default,
                    JoinRule = new ChannelRule
                    {
                        AllowedVocations = channel.Vocations,
                        MinMaxAllowedLevel = (channel.Level?.BiggerThan ?? 0, channel.Level?.LowerThan ?? 0)
                    },
                    WriteRule = new ChannelRule
                    {
                        AllowedVocations = channel.Write?.Vocations ?? default,
                        MinMaxAllowedLevel = (channel.Write?.Level?.BiggerThan ?? 0, channel.Write?.Level?.LowerThan ?? 0)
                    }
                });
            }

            logger.Verbose("Channels loaded!");
        }
    }
}
