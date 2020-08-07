using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Enums.Location;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;
using System.Collections.Generic;


namespace NeoServer.Server.Events
{
    public class PlayerTurnToDirectionEventHandler
    {
        private readonly IMap map;
        private readonly Game game;

        public PlayerTurnToDirectionEventHandler(IMap map, Game game)
        {
            this.map = map;
            this.game = game;
        }
        public void Execute(ICreature player, Direction direction)
        {

            var outgoingPackets = new Queue<IOutgoingPacket>();

            foreach (var spectatorId in map.GetPlayersAtPositionZone(player.Location))
            {
                var isSpectator = !(player.CreatureId == spectatorId);

                outgoingPackets.Enqueue(new TurnToDirectionPacket((IPlayer)player, direction));

                IConnection connection = null;
                if (!game.CreatureManager.GetPlayerConnection(spectatorId, out connection))
                {
                    continue;
                }

                connection.Send(outgoingPackets);


            }


        }
    }
}
