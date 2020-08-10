using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Enums;
using NeoServer.Game.Items;
using NeoServer.Game.World.Spawns;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Items;
using NeoServer.Server.Tasks;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Server.Events.Creature
{
    public class CreatureKilledEventHandler
    {
        private readonly IMap map;
        private readonly Game game;

        public CreatureKilledEventHandler(IMap map, Game game)
        {
            this.map = map;
            this.game = game;
        }
        public void Execute(ICreature creature)
        {
            game.Scheduler.AddEvent(new SchedulerEvent(600, () =>
            {
                var tile = creature.Tile;

                var thing = creature as IMoveableThing;
                map.RemoveThing(ref thing, tile);

                var corpse = ItemFactory.Create(creature.Corpse, creature.Location, null) as IMoveableThing;

                map.AddItem(ref corpse, tile);
            }));

            if (creature is IMonster monster)
            {
                game.CreatureManager.AddKilledMonsters(creature as IMonster);
            }


            //var outgoingPackets = new Queue<IOutgoingPacket>();

            //foreach (var spectatorId in map.GetPlayersAtPositionZone(creature.Location))
            //{

            //    if (creature.CreatureId == spectatorId)
            //    {
            //        //outgoingPackets.Enqueue(new TextMessagePacket($"{victim.Name} loses {healthDamageString} due to your attack", TextMessageOutgoingType.MESSAGE_STATUS_DEFAULT));
            //    }

            //    outgoingPackets.Enqueue(new RemoveTileThingPacket(tile, 1));
            //    //outgoingPackets.Enqueue(new AnimatedTextPacket(victim.Location, TextColor.Red, healthDamageString));
            //    //outgoingPackets.Enqueue(new CreatureHealthPacket(victim));

            //    IConnection connection = null;
            //    if (!game.CreatureManager.GetPlayerConnection(spectatorId, out connection))
            //    {
            //        continue;
            //    }

            //    connection.Send(outgoingPackets);


            //}
        }
    }
}
