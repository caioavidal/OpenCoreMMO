using NeoServer.Game.Common.Talks;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Chats;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.DataStore;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeoServer.Game.Chats
{
    public class ChatChannelFactory
    {
        public IChatChannel Create(Type type, string name, IPlayer player = null)
        {
            if (!typeof(IChatChannel).IsAssignableFrom(type)) return default;

            var id = typeof(PersonalChatChannel).IsAssignableTo(type) && player is not null ? GeneratePlayerUniqueId(player) : GenerateUniqueId();
         
            var channel = (IChatChannel) Activator.CreateInstance(type: type, id, name);

            SubscribeEvents(channel);

            return channel;
        }

        public IChatChannel CreateGuildChannel(string name, ushort guildId)
        {
            var id = GenerateUniqueId();
            var channel = new GuildChatChannel(id, name, guildId);
            SubscribeEvents(channel);
            return channel;
        }

        public IChatChannel CreatePartyChannel(string name = "Party")
        {
            var id = GenerateUniqueId();

            var channel = new ChatChannel(id, name);

            SubscribeEvents(channel);

            return channel;
        }
        public IChatChannel Create(string name, string description, bool opened, SpeechType chatColor, Dictionary<byte, SpeechType> chatColorByVocation, ChannelRule joinRule, ChannelRule writeRule, MuteRule muteRule)
        {
            var id = GenerateUniqueId();

            var channel = new ChatChannel(id, name)
            {
                Description = description,
                ChatColor = chatColor == SpeechType.None ? SpeechType.ChannelYellowText : chatColor,
                ChatColorByVocation = chatColorByVocation ?? default,
                JoinRule = joinRule,
                WriteRule = writeRule,
                MuteRule = muteRule.None ? MuteRule.Default : muteRule,
                Opened = opened
            };

            SubscribeEvents(channel);

            return channel;
        }

        //injected
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
        private static ushort GeneratePlayerUniqueId(IPlayer player)
        {
            ushort id;
            do
            {
                id = GenerateUniqueId();
            }
            while ((player.PersonalChannels?.Any(x=>x.Id == id) ?? false) || (player.PrivateChannels?.Any(x=>x.Id == id)?? false));

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
