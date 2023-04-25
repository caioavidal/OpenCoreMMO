using System.Collections.Generic;
using System.Linq;
using NeoServer.Data.Interfaces;
using NeoServer.Loaders.Interfaces;
using NeoServer.Networking.Packets.Incoming.Chat;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;
using NeoServer.Server.Tasks;

namespace NeoServer.Networking.Handlers.Chat;

public class PlayerAddVipHandler : PacketHandler
{
    private readonly IAccountRepository _accountRepository;
    private readonly IGameServer _game;
    private readonly IEnumerable<IPlayerLoader> _playerLoaders;

    public PlayerAddVipHandler(IGameServer game, IAccountRepository accountRepository,
        IEnumerable<IPlayerLoader> playerLoaders)
    {
        _game = game;
        _accountRepository = accountRepository;
        _playerLoaders = playerLoaders;
    }

    public override async void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        var addVipPacket = new AddVipPacket(message);

        if (addVipPacket.Name?.Length > 20) return;
        if (!_game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;

        if (!_game.CreatureManager.TryGetPlayer(addVipPacket.Name, out var vipPlayer))
        {
            var playerRecord = await _accountRepository.GetPlayer(addVipPacket.Name);
            if (_playerLoaders.FirstOrDefault(x => x.IsApplicable(playerRecord)) is not IPlayerLoader playerLoader)
                return;

            vipPlayer = playerLoader.Load(playerRecord);
        }

        if (vipPlayer is null)
        {
            connection.Send(new TextMessagePacket("A player with this name does not exist.",
                TextMessageOutgoingType.Small));
            return;
        }

        //todo: check if player can be added to vip list

        _game.Dispatcher.AddEvent(new Event(() => player.Vip.AddToVip(vipPlayer)));
    }
}