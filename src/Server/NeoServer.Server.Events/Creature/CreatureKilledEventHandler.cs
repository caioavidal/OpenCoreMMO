using NeoServer.Data.Interfaces;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Networking.Packets.Outgoing.Login;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Tasks;

namespace NeoServer.Server.Events.Creature;

public class CreatureKilledEventHandler
{
    private readonly IGameServer game;
    private readonly IAccountRepository accountRepository;

    public CreatureKilledEventHandler(IGameServer game, IAccountRepository accountRepository)
    {
        this.game = game;
        this.accountRepository = accountRepository;
    }

    public void Execute(ICombatActor creature, IThing by, ILoot loot)
    {
        game.Scheduler.AddEvent(new SchedulerEvent(200, () =>
        {
            //send packets to killed player
            if (creature is not IPlayer player ||
                !game.CreatureManager.GetPlayerConnection(creature.CreatureId, out var connection)) return;

            player.SetNewLocation(new Location(player.Town.Coordinate));
            accountRepository.UpdatePlayer(player);

            connection.OutgoingPackets.Enqueue(new ReLoginWindowOutgoingPacket());
            connection.Send();
        }));

        if (creature is IMonster { IsSummon: false } monster)
            game.CreatureManager.AddKilledMonsters(monster);
    }
}