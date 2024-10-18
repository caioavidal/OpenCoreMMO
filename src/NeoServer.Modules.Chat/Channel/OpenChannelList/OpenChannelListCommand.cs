using Mediator;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Networking.Packets.Network;
using NeoServer.Networking.Packets.Outgoing.Chat;

namespace NeoServer.Modules.Chat.Channel.OpenChannelList;

public record OpenChannelListCommand(IPlayer Player, IConnection Connection) : ICommand;

public class OpenChannelListCommandHandler(IChatChannelStore chatChannelStore)
    : ICommandHandler<OpenChannelListCommand>
{
    public ValueTask<Unit> Handle(OpenChannelListCommand command, CancellationToken cancellationToken)
    {
        var (player, connection) = command;

        var channels = chatChannelStore.All.Where(x => x.PlayerCanJoin(player));
        channels = player.Channels.PersonalChannels is null
            ? channels
            : channels.Concat(player.Channels.PersonalChannels);
        channels = player.Channels.PrivateChannels is not { } privateChannels
            ? channels
            : channels.Concat(privateChannels);

        connection.OutgoingPackets.Enqueue(new PlayerChannelListPacket(channels.ToArray()));
        connection.Send();
        
        return Unit.ValueTask;
    }
}