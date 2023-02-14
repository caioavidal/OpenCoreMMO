using System;
using System.Collections.Generic;
using System.Linq;
using NeoServer.Game.Chats.Rules;
using NeoServer.Game.Common.Chats;
using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Chats;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Common.Helpers;

namespace NeoServer.Game.Chats;

public class ChatChannelFactory
{
    //injected
    public IEnumerable<IChatChannelEventSubscriber> ChannelEventSubscribers { get; set; }
    public IChatChannelStore ChatChannelStore { get; set; }
    public IGuildStore GuildStore { get; set; }

    public IChatChannel Create(Type type, string name, IPlayer player = null)
    {
        if (!typeof(IChatChannel).IsAssignableFrom(type)) return default;

        var id = typeof(PersonalChatChannel).IsAssignableTo(type) && player is not null
            ? GeneratePlayerUniqueId(player)
            : GenerateUniqueId();

        var channel = (IChatChannel)Activator.CreateInstance(type, id, name);

        SubscribeEvents(channel);

        return channel;
    }

    public IChatChannel CreateGuildChannel(string name, ushort guildId)
    {
        var id = GenerateUniqueId();
        var guid = GuildStore.Get(guildId);
        var channel = new GuildChatChannel(id, name, guid);
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

    public IChatChannel Create(string name, string description, bool opened, SpeechType chatColor,
        Dictionary<byte, SpeechType> chatColorByVocation, ChannelRule joinRule, ChannelRule writeRule,
        MuteRule muteRule)
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

    private ushort GenerateUniqueId()
    {
        ushort id;
        do
        {
            id = RandomIdGenerator.Generate(ushort.MaxValue);
        } while (ChatChannelStore.Contains(id));

        return id;
    }

    private ushort GeneratePlayerUniqueId(IPlayer player)
    {
        ushort id;
        do
        {
            id = GenerateUniqueId();
        } while ((player.Channels.PersonalChannels?.Any(x => x.Id == id) ?? false) ||
                 (player.Channels.PrivateChannels?.Any(x => x.Id == id) ?? false));

        return id;
    }

    //todo: move this method to a base factory to be used in other factories
    private void SubscribeEvents(IChatChannel createdChannel)
    {
        foreach (var gameSubscriber in ChannelEventSubscribers.Where(x =>
                     x.GetType().IsAssignableTo(typeof(IGameEventSubscriber)))) //register game events first
            gameSubscriber.Subscribe(createdChannel);

        foreach (var subscriber in ChannelEventSubscribers.Where(x =>
                     !x.GetType().IsAssignableTo(typeof(IGameEventSubscriber)))) //than register server events
            subscriber.Subscribe(createdChannel);
    }
}