using NeoServer.Application.Features.Decay;
using NeoServer.Application.Infrastructure.Thread;
using NeoServer.Data.Interfaces;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Networking.Packets.Outgoing.Login;
using NeoServer.Server.Common.Contracts;

namespace NeoServer.Server.Events.Creature;

public class CreatureKilledEventHandler
{
    private readonly IGameServer _game;
    private readonly IPlayerRepository _playerRepository;
    private readonly IItemDecayTracker _itemDecayTracker;

    public CreatureKilledEventHandler(IGameServer game, IPlayerRepository playerRepository, IItemDecayTracker itemDecayTracker )
    {
        _game = game;
        _playerRepository = playerRepository;
        _itemDecayTracker = itemDecayTracker;
    }

    public void Execute(ICombatActor creature, IThing by, ILoot loot)
    {
        if (creature.Corpse is IItem corpse) _itemDecayTracker.Track(corpse);
        
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