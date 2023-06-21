using System.Collections.Generic;
using System.Linq;
using NeoServer.Data.Interfaces;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Loaders.Interfaces;
using NeoServer.Networking.Packets.Incoming.Chat;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Commands;
using NeoServer.Server.Common.Contracts.Network;
using NeoServer.Server.Tasks;

namespace NeoServer.Server.Commands.Player;

public class AddPlayerToVipCommand : ICommand
{
    private readonly IGameServer _gameServer;
    private readonly IAccountRepository _accountRepository;
    private readonly IEnumerable<IPlayerLoader> _playerLoaders;

    public AddPlayerToVipCommand(IGameServer gameServer, IAccountRepository accountRepository,
        IEnumerable<IPlayerLoader> playerLoaders)
    {
        _gameServer = gameServer;
        _accountRepository = accountRepository;
        _playerLoaders = playerLoaders;
    }

    public void Execute(AddVipPacket addVipPacket, IConnection connection)
    {
        if (addVipPacket.Name?.Length > 20) return;
        if (!_gameServer.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;

        var vipPlayer = GetVipPlayer(addVipPacket);

        if (vipPlayer is null)
        {
            connection.Send(new TextMessagePacket("A player with this name does not exist.",
                TextMessageOutgoingType.Small));
            return;
        }
        
        _gameServer.Dispatcher.AddEvent(new Event(() => player.Vip.AddToVip(vipPlayer)));
    }

    private IPlayer GetVipPlayer(AddVipPacket addVipPacket)
    {
        if (_gameServer.CreatureManager.TryGetPlayer(addVipPacket.Name, out var vipPlayer)) return vipPlayer;

        var playerRecord = _accountRepository.GetPlayer(addVipPacket.Name).Result;
        if (playerRecord is null) return null;

        if (_playerLoaders.FirstOrDefault(x => x.IsApplicable(playerRecord)) is not { } playerLoader)
            return null;

        vipPlayer = playerLoader.Load(playerRecord);

        return vipPlayer;
    }
}