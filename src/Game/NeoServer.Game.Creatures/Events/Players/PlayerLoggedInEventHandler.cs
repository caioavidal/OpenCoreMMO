using System.Linq;
using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.DataStores;

namespace NeoServer.Game.Creatures.Events.Players;

public class PlayerLoggedInEventHandler : IGameEventHandler
{
    private readonly IChatChannelStore _chatChannelStore;
    private readonly IGuildStore _guildStore;

    public PlayerLoggedInEventHandler(IChatChannelStore chatChannelStore, IGuildStore guildStore)
    {
        _chatChannelStore = chatChannelStore;
        _guildStore = guildStore;
    }

    public void Execute(IPlayer player)
    {
        if (player is null) return;

        var channels = _chatChannelStore.All.Where(x => x.Opened);

        channels = player.Channels.PersonalChannels is null
            ? channels
            : channels.Concat(player.Channels.PersonalChannels?.Where(x => x.Opened));

        channels = player.Channels.PrivateChannels is not { } privateChannels
            ? channels
            : channels.Concat(privateChannels.Where(x => x.Opened));

        foreach (var channel in channels) player.Channels.JoinChannel(channel);
    }
}