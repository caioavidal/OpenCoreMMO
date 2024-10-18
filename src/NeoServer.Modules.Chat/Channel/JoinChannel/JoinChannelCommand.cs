using Mediator;
using NeoServer.Game.Chat.Channels.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.DataStores;

namespace NeoServer.Modules.Chat.Channel.JoinChannel;

public record JoinChannelCommand(IPlayer Player, ushort ChannelId) : ICommand;

public class JoinChannelCommandHandler(IChatChannelStore chatChannelStore) : ICommandHandler<JoinChannelCommand>
{
    public ValueTask<Unit> Handle(JoinChannelCommand command, CancellationToken cancellationToken)
    {
        var (player, channelId) = command;
        
        IChatChannel channel = null;
        if (chatChannelStore.Get(channelId) is { } publicChannel)
            channel = publicChannel;
        if (player.Channels.PersonalChannels?.FirstOrDefault(x => x.Id == channelId) is
            { } personalChannel) channel = personalChannel;
        if (player.Channels.PrivateChannels?.FirstOrDefault(x => x.Id == channelId) is
            { } privateChannel) channel = privateChannel;

        if (channel is null) return Unit.ValueTask;

        player.Channels.JoinChannel(channel);
        return Unit.ValueTask;
    }
}