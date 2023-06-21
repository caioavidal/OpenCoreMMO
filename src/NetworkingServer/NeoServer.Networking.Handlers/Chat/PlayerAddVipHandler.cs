using NeoServer.Networking.Packets.Incoming.Chat;
using NeoServer.Server.Commands.Player;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;
using NeoServer.Server.Tasks;

namespace NeoServer.Networking.Handlers.Chat;

public class PlayerAddVipHandler : PacketHandler
{
    private readonly IGameServer _game;
    private readonly AddPlayerToVipCommand _playerToVipCommand;

    public PlayerAddVipHandler(IGameServer game, AddPlayerToVipCommand playerToVipCommand)
    {
        _game = game;
        _playerToVipCommand = playerToVipCommand;
    }

    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        var addVipPacket = new AddVipPacket(message);

        _game.Dispatcher.AddEvent(new Event(() => _playerToVipCommand.Execute(addVipPacket, connection)));
    }
}