using NeoServer.BuildingBlocks.Application.Contracts;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Spells;
using NeoServer.Game.Common.Creatures;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Networking.Packets.Outgoing.Effect;

namespace NeoServer.Modules.Players.Events;

public class PlayerCannotUseSpellEventHandler : IEventHandler
{
    private readonly IGameServer game;

    public PlayerCannotUseSpellEventHandler(IGameServer game)
    {
        this.game = game;
    }

    public void Execute(ICreature creature, ISpell spell, InvalidOperation error)
    {
        foreach (var spectator in game.Map.GetPlayersAtPositionZone(creature.Location))
        {
            if (!game.CreatureManager.GetPlayerConnection(spectator.CreatureId, out var connection)) continue;

            connection.OutgoingPackets.Enqueue(new MagicEffectPacket(creature.Location, EffectT.Puff));

            var msg = TextMessageOutgoingParser.Parse(error);

            if (!string.IsNullOrEmpty(msg))
                connection.OutgoingPackets.Enqueue(new TextMessagePacket(msg,
                    TextMessageOutgoingType.MESSAGE_STATUS_DEFAULT));

            connection.Send();
        }
    }
}