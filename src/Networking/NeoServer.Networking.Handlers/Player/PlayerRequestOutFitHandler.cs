using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Networking.Packets.Outgoing.Player;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;

namespace NeoServer.Networking.Handlers.Player;

public class PlayerRequestOutFitHandler : PacketHandler
{
    private readonly IGameServer game;
    private readonly IPlayerOutFitStore _playerOutFitStore;

    public PlayerRequestOutFitHandler(IGameServer game, IPlayerOutFitStore playerOutFitStore)
    {
        this.game = game;
        _playerOutFitStore = playerOutFitStore;
    }
    
    public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        if (!game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;

        var outfits = _playerOutFitStore.Get(player.Gender);
        
        connection.OutgoingPackets.Enqueue(new PlayerOutFitWindowPacket(player, outfits));
        connection.Send();
    }
}