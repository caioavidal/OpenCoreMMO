using NeoServer.BuildingBlocks.Application.Contracts;
using NeoServer.BuildingBlocks.Infrastructure.Threading.Event;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Networking.Packets.Network;

namespace NeoServer.PacketHandler.Modules.Players.ChangeMode;

public class PlayerChangesModePacketHandler : PacketHandler
{
    private readonly IGameServer _game;

    public PlayerChangesModePacketHandler(IGameServer game)
    {
        _game = game;
    }

    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        var changeMode = new ChangeModePacket(message);

        if (!_game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;

        _game.Dispatcher.AddEvent(new Event(() =>
        {
            player.ChangeFightMode(changeMode.FightMode);
            player.ChangeChaseMode(changeMode.ChaseMode);
            player.ChangeSecureMode(changeMode.SecureMode);
        }));
    }
}