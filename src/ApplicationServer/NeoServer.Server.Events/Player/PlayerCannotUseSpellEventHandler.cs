using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Spells;
using NeoServer.Game.Common.Creatures;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Networking.Packets.Outgoing.Effect;
using NeoServer.Server.Common.Contracts;

namespace NeoServer.Server.Events.Player;

public class PlayerCannotUseSpellEventHandler
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
            connection.OutgoingPackets.Enqueue(new TextMessagePacket(TextMessageOutgoingParser.Parse(error),
                TextMessageOutgoingType.MESSAGE_STATUS_DEFAULT));
            connection.Send();
        }
    }
}