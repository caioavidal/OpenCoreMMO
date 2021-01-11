using NeoServer.Game.Common.Parsers;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Networking.Packets.Outgoing;

namespace NeoServer.Server.Events.Player
{
    public class PlayerConditionChangedEventHandler
    {
        private readonly Game game;

        public PlayerConditionChangedEventHandler(Game game)
        {
            this.game = game;
        }

        public void Execute(ICreature creature, ICondition c)
        {
            ushort icons = 0;
            foreach (var condition in creature.Conditions)
            {
                icons |= (ushort)ConditionIconParser.Parse(condition.Key);
            }

            if (game.CreatureManager.GetPlayerConnection(creature.CreatureId, out var connection))
            {
                connection.OutgoingPackets.Enqueue(new ConditionIconPacket(icons));
                connection.Send();
            }
        }

   
    }
}