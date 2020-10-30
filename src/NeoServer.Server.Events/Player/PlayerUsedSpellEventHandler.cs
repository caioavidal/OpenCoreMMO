using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Spells;
using NeoServer.Game.Enums.Location;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;

namespace NeoServer.Server.Events
{
    public class PlayerUsedSpellEventHandler
    {
        private readonly Game game;

        public PlayerUsedSpellEventHandler(Game game)
        {
            this.game = game;
        }
        public void Execute(ICreature creature, ISpell spell)
        {
            foreach (var spectatorId in game.Map.GetPlayersAtPositionZone(creature.Location))
            {
                if (!game.CreatureManager.GetPlayerConnection(spectatorId, out IConnection connection))
                {
                    continue;
                }

                connection.OutgoingPackets.Enqueue(new MagicEffectPacket(creature.Location, spell.Effect));
                connection.Send();
            }
        }
    }
}
