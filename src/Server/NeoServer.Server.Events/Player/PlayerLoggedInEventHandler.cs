using NeoServer.Data.Interfaces;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Helpers;
using NeoServer.Server.Common.Contracts;

namespace NeoServer.Server.Events.Player;

public class PlayerLoggedInEventHandler : IEventHandler
{
    private readonly IAccountRepository _accountRepository;
    private readonly IGameServer game;
    private readonly IMap map;

    public PlayerLoggedInEventHandler(IMap map, IGameServer game, IAccountRepository accountRepository)
    {
        this.map = map;
        this.game = game;
        _accountRepository = accountRepository;
    }

    public async void Execute(IWalkableCreature creature)
    {
        if (creature.IsNull()) return;

        if (creature is not IPlayer player) return;

        await _accountRepository.UpdatePlayerOnlineStatus(player.Id, true);
    }
}