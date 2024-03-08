using NeoServer.Application.Common.Contracts;
using NeoServer.Application.Common.Contracts.Repositories;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Networking.Packets.Outgoing.Chat;

namespace NeoServer.Application.Features.Chat.Vip.AddFriendToVip;

public class AddedFriendToVipListEventHandler : IEventHandler
{
    private readonly IAccountRepository _accountRepository;
    private readonly IGameServer _game;

    public AddedFriendToVipListEventHandler(IGameServer game, IAccountRepository accountRepository)
    {
        _game = game;
        _accountRepository = accountRepository;
    }

    public async void Execute(IPlayer player, uint vipPlayerId, string vipPlayerName)
    {
        if (!_game.CreatureManager.GetPlayerConnection(player.CreatureId, out var connection)) return;

        await _accountRepository.AddPlayerToVipList((int)player.AccountId, (int)vipPlayerId);

        var isOnline = _game.CreatureManager.TryGetLoggedPlayer(vipPlayerId, out _);

        connection.OutgoingPackets.Enqueue(new PlayerAddVipPacket(vipPlayerId, vipPlayerName, isOnline));
        connection.Send();
    }
}