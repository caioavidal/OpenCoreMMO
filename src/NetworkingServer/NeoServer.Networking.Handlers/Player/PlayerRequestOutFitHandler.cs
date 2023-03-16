using System.Linq;
using NeoServer.Data.Contexts;
using NeoServer.Data.Interfaces;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Common.Helpers;
using NeoServer.Networking.Packets.Outgoing.Player;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;

namespace NeoServer.Networking.Handlers.Player;

public class PlayerRequestOutFitHandler : PacketHandler
{
    private readonly IPlayerOutFitStore _playerOutFitStore;
    private readonly IPlayerRepository _playerRepository;
    private readonly IGameServer game;

    public PlayerRequestOutFitHandler(IGameServer game, IPlayerOutFitStore playerOutFitStore, IPlayerRepository playerRepository)
    {
        this.game = game;
        _playerOutFitStore = playerOutFitStore;
        _playerRepository = playerRepository;
    }

    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        if (!game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;

        if (player.IsNull()) return;

        var outfits = _playerOutFitStore.Get(player.Gender);

        var playerAddons = _playerRepository.GetOutfitAddons((int)player.Id).Result;

        connection.OutgoingPackets.Enqueue(new PlayerOutFitWindowPacket(player, outfits, playerAddons));
        connection.Send();
    }
}