using Mediator;
using NeoServer.BuildingBlocks.Application.Contracts;
using NeoServer.BuildingBlocks.Infrastructure.Threading.Event;
using NeoServer.Modules.Chat.PlayerSay;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Networking.Packets.Network;

namespace NeoServer.PacketHandler.Modules.Chat.PlayerSay;

public class PlayerSayPacketHandler : PacketHandler
{
    private readonly IGameServer _game;
    private readonly IMediator _mediator;

    public PlayerSayPacketHandler(IGameServer game, IMediator mediator)
    {
        _game = game;
        _mediator = mediator;
    }

    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        var playerSay = new PlayerSayPacket(message);
        if (!_game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;

        var command = new PlayerSayCommand(player, connection, playerSay.TalkType, playerSay.Receiver,
            playerSay.Message, playerSay.ChannelId);

        _game.Dispatcher.AddEvent(new Event(() => _ = _mediator.Send(command)));
    }
}