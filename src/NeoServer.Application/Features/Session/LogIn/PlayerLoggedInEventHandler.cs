using NeoServer.Application.Common.Contracts;
using NeoServer.Application.Common.Contracts.Repositories;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Helpers;

namespace NeoServer.Application.Features.Session.LogIn;

public class PlayerLoggedInEventHandler : IEventHandler
{
    private readonly IPlayerRepository _playerRepository;

    public PlayerLoggedInEventHandler(IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
    }

    public async void Execute(IWalkableCreature creature)
    {
        if (creature.IsNull()) return;

        if (creature is not IPlayer player) return;

        await _playerRepository.UpdatePlayerOnlineStatus(player.Id, true);
    }
}