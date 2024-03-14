using NeoServer.Application.Common.Contracts;
using NeoServer.Application.Common.Contracts.Repositories;
using NeoServer.Application.Features.Item.Decay;
using NeoServer.Application.Infrastructure.Thread;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Networking.Packets.Outgoing.Login;

namespace NeoServer.Application.Features.Creature.Events;

public class CreatureKilledEventHandler : IEventHandler
{
    private readonly IGameServer _game;
    private readonly IItemDecayTracker _itemDecayTracker;
    private readonly IPlayerRepository _playerRepository;

    public CreatureKilledEventHandler(IGameServer game, IPlayerRepository playerRepository,
        IItemDecayTracker itemDecayTracker)
    {
        _game = game;
        _playerRepository = playerRepository;
        _itemDecayTracker = itemDecayTracker;
    }

    public void Execute(ICombatActor creature, IThing by, ILoot loot)
    {
        _game.Scheduler.AddEvent(new SchedulerEvent(200, () =>
        {
            //send packets to killed player
            if (creature is not IPlayer player ||
                !_game.CreatureManager.GetPlayerConnection(creature.CreatureId, out var connection)) return;

            _playerRepository.SavePlayer(player);

            connection.OutgoingPackets.Enqueue(new ReLoginWindowOutgoingPacket());
            connection.Send();
        }));

        OnMonsterKilled(creature);
    }

    private void OnMonsterKilled(ICombatActor creature)
    {
        if (creature is not IMonster { IsSummon: false } monster) return;

        _game.CreatureManager.AddKilledMonsters(monster);
    }
}