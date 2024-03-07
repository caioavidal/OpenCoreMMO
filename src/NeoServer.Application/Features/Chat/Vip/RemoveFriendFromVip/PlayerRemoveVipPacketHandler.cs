using NeoServer.Application.Common.Contracts;
using NeoServer.Application.Common.Contracts.Network;
using NeoServer.Application.Common.PacketHandler;
using NeoServer.Application.Infrastructure.Thread;
using NeoServer.Application.Repositories;
using NeoServer.Networking.Packets.Incoming.Chat;
using NeoServer.Server.Common.Contracts.Network;

namespace NeoServer.Application.Features.Chat.Vip.RemoveFriendFromVip;

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