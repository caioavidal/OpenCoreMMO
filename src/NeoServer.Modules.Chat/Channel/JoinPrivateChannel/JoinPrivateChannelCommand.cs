using Mediator;
using NeoServer.BuildingBlocks.Application.Contracts;
using NeoServer.BuildingBlocks.Application.Contracts.Repositories;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Networking.Packets.Network;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Networking.Packets.Outgoing.Chat;

namespace NeoServer.Modules.Chat.Channel.JoinPrivateChannel;

public record JoinPrivateChannelCommand(IPlayer Player, string Receiver, IConnection Connection) : ICommand;

public class JoinPrivateChannelCommandHandler(IChatChannelStore chatChannelStore, IPlayerRepository playerRepository,
    IGameCreatureManager creatureManager) : ICommandHandler<JoinPrivateChannelCommand>
{
    public async ValueTask<Unit> Handle(JoinPrivateChannelCommand command, CancellationToken cancellationToken)
    {

        var (player, receiver, connection) = command;
        
        if (string.IsNullOrWhiteSpace(receiver) ||
            await playerRepository.GetPlayer(receiver) is null)
        {
            connection.Send(new TextMessagePacket("A player with this name does not exist.",
                TextMessageOutgoingType.Small));
            return Unit.Value;
        }

        if (string.IsNullOrWhiteSpace(receiver) ||
            !creatureManager.TryGetPlayer(receiver, out var receipt))
        {
            connection.Send(new TextMessagePacket("A player with this name is not online.",
                TextMessageOutgoingType.Small));
            return Unit.Value;
        }

        if (receiver.Trim().Equals(player.Name.Trim(), StringComparison.InvariantCultureIgnoreCase))
        {
            connection.Send(new TextMessagePacket("You cannot set up a private message channel with yourself.",
                TextMessageOutgoingType.Small));
            return Unit.Value;
        }

        if (receipt is null) return Unit.Value;

        connection.OutgoingPackets.Enqueue(new PlayerOpenPrivateChannelPacket(receipt.Name));
        connection.Send();
        return Unit.Value;
    }
}