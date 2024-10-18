using NeoServer.BuildingBlocks.Application.Contracts;
using NeoServer.BuildingBlocks.Application.Contracts.Repositories;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Common.Helpers;
using NeoServer.Networking.Packets.Network;
using NeoServer.Networking.Packets.Outgoing.Player;

namespace NeoServer.PacketHandler.Features.Player.Outfit;

public class PlayerRequestOutfitPacketHandler : PacketHandler
{
    private readonly IGameServer _game;
    private readonly IPlayerOutFitStore _playerOutFitStore;
    private readonly IPlayerRepository _playerRepository;

    public PlayerRequestOutfitPacketHandler(IGameServer game, IPlayerOutFitStore playerOutFitStore,
        IPlayerRepository playerRepository)
    {
        _game = game;
        _playerOutFitStore = playerOutFitStore;
        _playerRepository = playerRepository;
    }

    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        if (!_game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;

        if (player.IsNull()) return;

        var outfits = _playerOutFitStore.Get(player.Gender);

        var playerAddons = _playerRepository.GetOutfitAddons((int)player.Id).Result;

        connection.OutgoingPackets.Enqueue(new PlayerOutFitWindowPacket(player, outfits, playerAddons));
        connection.Send();
    }
}