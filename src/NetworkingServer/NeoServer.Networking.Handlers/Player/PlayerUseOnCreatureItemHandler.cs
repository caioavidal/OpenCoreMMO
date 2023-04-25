using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Commands.Player.UseItem;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;
using NeoServer.Server.Tasks;

namespace NeoServer.Networking.Handlers.Player;

public class PlayerUseOnCreatureHandler : PacketHandler
{
    private readonly IGameServer _game;
    private readonly PlayerUseItemOnCreatureCommand _playerUseItemOnCreatureCommand;

    public PlayerUseOnCreatureHandler(IGameServer game,
        PlayerUseItemOnCreatureCommand playerUseItemOnCreatureCommand)
    {
        _game = game;
        _playerUseItemOnCreatureCommand = playerUseItemOnCreatureCommand;
    }

    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        var useItemOnPacket = new UseItemOnCreaturePacket(message);
        if (_game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player))
            _game.Dispatcher.AddEvent(new Event(2000,
                () => _playerUseItemOnCreatureCommand.Execute(player,
                    useItemOnPacket))); //todo create a const for 2000 expiration time
    }
}