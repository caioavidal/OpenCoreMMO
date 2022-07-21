using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;

namespace NeoServer.Networking.Handlers.Player;

public class PlayerChangeCompletedOutFitHandler : PacketHandler
{
    private readonly IGameServer game;
    private readonly IPlayerOutFitStore _playerOutFitStore;

    public PlayerChangeCompletedOutFitHandler(IGameServer game, IPlayerOutFitStore playerOutFitStore)
    {
        this.game = game;
        _playerOutFitStore = playerOutFitStore;
    }
    
    public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        if (!game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;
    }
}