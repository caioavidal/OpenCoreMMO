using NeoServer.Game.Common;
using NeoServer.Game.Common.Talks;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Chats;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.DataStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Chats
{
    public class ChatChannelFactory
    {
        public IChatChannel Create(string name,string description, SpeechType chatColor, Dictionary<byte, SpeechType> chatColorByVocation, ChannelRule joinRule, ChannelRule writeRule, MuteRule muteRule)
        {
            var id = GenerateUniqueId();

            var channel = new ChatChannel(id, name)
            {
                Description = description,
                ChatColor = chatColor == SpeechType.None ? SpeechType.ChannelY : chatColor,
                ChatColorByVocation = chatColorByVocation ?? default,
                JoinRule = joinRule,
                WriteRule = writeRule,
                MuteRule = muteRule.None ? MuteRule.Default : muteRule
            };

            SubscribeEvents(channel);

            return channel;
        }
        public IEnumerable<IChatChannelEventSubscriber> ChannelEventSubscribers { get; set; }

        private static ushort GenerateUniqueId()
        {
            ushort id;
            do
            {
                id = RandomIdGenerator.Generate(ushort.MaxValue);
            }
            while (ChatChannelStore.Data.Contains(id));

            return id;
        }

        //todo: move this method to a base factory to be used in other factories
        private void SubscribeEvents(IChatChannel createdChannel)
        {
            foreach (var gameSubscriber in ChannelEventSubscribers.Where(x => x.GetType().IsAssignableTo(typeof(IGameEventSubscriber)))) //register game events first
            {
                gameSubscriber.Subscribe(createdChannel);
            }

            foreach (var subscriber in ChannelEventSubscribers.Where(x => !x.GetType().IsAssignableTo(typeof(IGameEventSubscriber)))) //than register server events
            {
                subscriber.Subscribe(createdChannel);
            }
        }

    }
}
