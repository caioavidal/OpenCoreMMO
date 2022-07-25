using System.Linq;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Networking.Packets.Incoming.Player;
using NeoServer.Networking.Packets.Outgoing.Creature;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;
using NeoServer.Server.Tasks;

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

        var packet = new PlayerChangeOutFitPacket(message);

        var outfitToChange = _playerOutFitStore.Get(player.Gender).FirstOrDefault(item => item.LookType == packet.Outfit.LookType);
        
        if (outfitToChange is null) return;

        var outfit = packet.Outfit
            .SetEnabled(outfitToChange.Enabled)
            .SetGender(outfitToChange.Type)
            .SetName(outfitToChange.Name)
            .SetPremium(outfitToChange.Premium)
            .SetUnlocked(outfitToChange.Unlocked);
        
        player.ChangeOutfit(outfit);
    }
}