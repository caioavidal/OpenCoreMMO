using NeoServer.BuildingBlocks.Application.Contracts;
using NeoServer.BuildingBlocks.Application.Contracts.Repositories;
using NeoServer.BuildingBlocks.Infrastructure.Threading.Event;
using NeoServer.Networking.Packets.Incoming.Chat;
using NeoServer.Networking.Packets.Network;

namespace NeoServer.PacketHandler.Modules.Chat.Vip.RemoveFriendFromVip;

public class PlayerRemoveVipPacketHandler : PacketHandler
{
    private readonly IAccountRepository _accountRepository;
    private readonly IGameServer _game;

    public PlayerRemoveVipPacketHandler(IGameServer game, IAccountRepository accountRepository)
    {
        _game = game;
        _accountRepository = accountRepository;
    }

    public override async void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        if (!_game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;

        var removeVipPacket = new RemoveVipPacket(message);

        _game.Dispatcher.AddEvent(new Event(() => player.Vip.RemoveFromVip(removeVipPacket.PlayerId)));

        await _accountRepository.RemoveFromVipList((int)player.AccountId, (int)removeVipPacket.PlayerId);
    }
}