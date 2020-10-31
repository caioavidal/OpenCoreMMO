using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Spells;
using NeoServer.Game.Enums.Location;
using NeoServer.Game.Enums.Spells;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;

namespace NeoServer.Server.Events
{
    public class PlayerCannotUseSpellEventHandler
    {
        private readonly Game game;

        public PlayerCannotUseSpellEventHandler(Game game)
        {
            this.game = game;
        }
        public void Execute(ICreature creature, ISpell spell, SpellError error)
        {
            foreach (var spectatorId in game.Map.GetPlayersAtPositionZone(creature.Location))
            {
                if (!game.CreatureManager.GetPlayerConnection(spectatorId, out IConnection connection))
                {
                    continue;
                }

                connection.OutgoingPackets.Enqueue(new MagicEffectPacket(creature.Location, EffectT.Puff));
                connection.OutgoingPackets.Enqueue(new TextMessagePacket(SpellMessage.GetMessage(error), TextMessageOutgoingType.MESSAGE_STATUS_DEFAULT));
                connection.Send();
            }
        }
    }
}
