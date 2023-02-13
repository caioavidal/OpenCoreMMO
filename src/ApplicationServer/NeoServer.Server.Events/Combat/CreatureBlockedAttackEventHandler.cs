using NeoServer.Game.Common.Combat;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Creatures;
using NeoServer.Networking.Packets.Outgoing.Effect;
using NeoServer.Server.Common.Contracts;

namespace NeoServer.Server.Events.Combat;

public class CreatureBlockedAttackEventHandler
{
    private readonly IGameServer game;

    public CreatureBlockedAttackEventHandler(IGameServer game)
    {
        this.game = game;
    }

    public void Execute(ICreature creature, BlockType blockType)
    {
        foreach (var spectator in game.Map.GetPlayersAtPositionZone(creature.Location))
        {
            var effect = blockType == BlockType.Armor ? EffectT.SparkYellow : EffectT.Puff;

            if (!game.CreatureManager.GetPlayerConnection(spectator.CreatureId, out var connection)) continue;

            connection.OutgoingPackets.Enqueue(new MagicEffectPacket(creature.Location, effect));
            connection.Send();
        }
    }
}