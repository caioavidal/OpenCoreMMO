using NeoServer.BuildingBlocks.Application.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Networking.Packets.Outgoing.Chat;

namespace NeoServer.Modules.Chat.Vip.LoadVipList;

public class PlayerLoadedVipListEventHandler : IEventHandler
{
    private readonly IGameServer _game;

    public PlayerLoadedVipListEventHandler(IGameServer game)
    {
        _game = game;
    }

    public void Execute(IPlayer player, IEnumerable<(uint, string)> vipList)
    {
        if (!_game.CreatureManager.GetPlayerConnection(player.CreatureId, out var connection)) return;

        foreach (var (playerId, playerName) in vipList)
        {
            var isOnline = _game.CreatureManager.TryGetLoggedPlayer(playerId, out _);
            connection.OutgoingPackets.Enqueue(new PlayerAddVipPacket(playerId, playerName, isOnline));
        }

        connection.Send();
    }
}