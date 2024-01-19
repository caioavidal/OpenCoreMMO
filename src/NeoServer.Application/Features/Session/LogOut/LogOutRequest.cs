using Mediator;
using NeoServer.Application.Common.PacketHandler;
using NeoServer.Infrastructure.Thread;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;

namespace NeoServer.Application.Features.Session.LogOut;

public class PlayerLogOutPacketHandler : PacketHandler
{
    private readonly IGameServer _game;
    private readonly IMediator _mediator;

    public PlayerLogOutPacketHandler(IGameServer game, IMediator mediator)
    {
        _game = game;
        _mediator = mediator;
    }

    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        if (!_game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;

        _game.Dispatcher.AddEvent(new Event(() => _ = _mediator.Send(new LogOutCommand(player, false))));
    }
}