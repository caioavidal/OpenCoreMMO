using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Commands.Player.UseItem;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;
using NeoServer.Server.Tasks;

namespace NeoServer.Networking.Handlers.Player;

public class PlayerUseItemHandler : PacketHandler
{
    private readonly IGameServer _game;
    private readonly PlayerUseItemCommand _playerUseItemCommand;

    public PlayerUseItemHandler(IGameServer game, PlayerUseItemCommand playerUseItemCommand)
    {
        _game = game;
        _playerUseItemCommand = playerUseItemCommand;
    }

    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        var useItemPacket = new UseItemPacket(message);

        if (!_game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;

        _game.Dispatcher.AddEvent(new Event(2000,
            () => _playerUseItemCommand.Execute(player,
                useItemPacket))); //todo create a const for 2000 expiration time
    }
}