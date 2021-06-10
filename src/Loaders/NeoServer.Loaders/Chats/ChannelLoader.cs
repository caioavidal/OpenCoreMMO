using System.Collections.Generic;
using System.IO;
using System.Linq;
using NeoServer.Game.Chats;
using NeoServer.Game.Chats.Rules;
using NeoServer.Game.Common.Chats;
using NeoServer.Game.Common.Contracts.Chats;
using NeoServer.Game.DataStore;
using NeoServer.Loaders.Interfaces;
using NeoServer.Server.Configurations;
using Newtonsoft.Json;
using Serilog.Core;

namespace NeoServer.Loaders.Chats
{
    public class ChannelLoader : IStartupLoader
    {
        private readonly ChatChannelFactory chatChannelFactory;
        private readonly Logger logger;
        private readonly ServerConfiguration serverConfiguration;

        public ChannelLoader(ServerConfiguration serverConfiguration, Logger logger,
            ChatChannelFactory chatChannelFactory)
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
                        channel.Color?.ByVocation?.ToDictionary(x => (byte) x.Key, x => ParseColor(x.Value)) ?? default,
                        new ChannelRule
                        {
                            AllowedVocations = channel.Vocations,
                            MinMaxAllowedLevel = (channel.Level?.BiggerThan ?? 0, channel.Level?.LowerThan ?? 0)
                        },
                        new ChannelRule
                        {
                            AllowedVocations = channel.Write?.Vocations ?? default,
                            MinMaxAllowedLevel = (channel.Write?.Level?.BiggerThan ?? 0,
                                channel.Write?.Level?.LowerThan ?? 0)
                        },
                        channel.MuteRule is null
                            ? default
                            : new MuteRule
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
                "red" => SpeechType.ChannelRed1Text,
                "yellow" => SpeechType.ChannelYellowText,
                "white" => SpeechType.ChannelWhiteText,
                "orange" => SpeechType.ChannelOrangeText,
                _ => SpeechType.ChannelYellowText
            };
        }
    }
}