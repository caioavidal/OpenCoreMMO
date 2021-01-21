using NeoServer.Game.Chats;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Talks;
using NeoServer.Game.Contracts.Chats;
using NeoServer.Game.DataStore;
using NeoServer.Loaders;
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
        private readonly ChatChannelFactory chatChannelFactory;
        public ChannelLoader(ServerConfiguration serverConfiguration, Logger logger, ChatChannelFactory chatChannelFactory)
        {
            this.serverConfiguration = serverConfiguration;
            this.logger = logger;
            this.chatChannelFactory = chatChannelFactory;
        }
        public void Load()
        {
            var jsonString = File.ReadAllText(Path.Combine(serverConfiguration.Data, "channels.json"));

            var channels = JsonConvert.DeserializeObject<List<ChannelModel>>(jsonString);

            foreach (var channel in channels.Where(x => x.Enabled))
            {
                IChatChannel createdChannel = null;
                if (!string.IsNullOrWhiteSpace(channel.Script))
                {
                    var type = ScriptSearch.Get(channel.Script);
                    createdChannel = chatChannelFactory.Create(type, channel.Name);
                }
                else
                {

                    createdChannel = chatChannelFactory.Create(channel.Name, channel.Description, channel.Opened,
                        ParseColor(channel.Color?.Default),
                        channel.Color?.ByVocation?.ToDictionary(x => (byte)x.Key, x => ParseColor(x.Value)) ?? default,
                        new ChannelRule
                        {
                            AllowedVocations = channel.Vocations,
                            MinMaxAllowedLevel = (channel.Level?.BiggerThan ?? 0, channel.Level?.LowerThan ?? 0)
                        },
                         new ChannelRule
                         {
                             AllowedVocations = channel.Write?.Vocations ?? default,
                             MinMaxAllowedLevel = (channel.Write?.Level?.BiggerThan ?? 0, channel.Write?.Level?.LowerThan ?? 0)
                         },
                         channel.MuteRule is null ? default : new MuteRule
                         {
                             CancelMessage = channel.MuteRule.CancelMessage,
                             MessagesCount = channel.MuteRule.MessagesCount,
                             TimeMultiplier = channel.MuteRule.TimeMultiplier,
                             TimeToBlock = channel.MuteRule.TimeToBlock,
                             WaitTime = channel.MuteRule.WaitTime
                         });
                }

                if (createdChannel is null) continue;

                ChatChannelStore.Data.Add(createdChannel.Id, createdChannel);
            }

            logger.Verbose("Channels loaded!");
        }

        private SpeechType ParseColor(string color)
        {
            return color switch
            {
                "red" => SpeechType.ChannelR1,
                "yellow" => SpeechType.ChannelY,
                "white" => SpeechType.ChannelW,
                "orange" => SpeechType.ChannelO,
                _ => SpeechType.ChannelY
            };
        }
    }
}
