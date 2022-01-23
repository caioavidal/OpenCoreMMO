using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Commands.Player;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;
using NeoServer.Server.Tasks;

namespace NeoServer.Networking.Handlers.Chat;

public class PlayerSayHandler : PacketHandler
{
    private readonly IGameServer game;
    private readonly PlayerSayCommand playerSayCommand;

    public PlayerSayHandler(IGameServer game, PlayerSayCommand playerSayCommand)
    {
        this.game = game;
        this.playerSayCommand = playerSayCommand;
    }

    public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        var playerSay = new PlayerSayPacket(message);
        if (!game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;

        game.Dispatcher.AddEvent(new Event(() => playerSayCommand.Execute(player, connection, playerSay)));
    }
}