using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Server.Common.Contracts.Network;

namespace NeoServer.Networking.Packets.Outgoing.Creature;

public class CreatureOutfitPacket : OutgoingPacket
{
    private ICreature Creature { get; }

    public CreatureOutfitPacket(ICreature creature) => Creature = creature;

    public override void WriteToMessage(INetworkMessage message)
    {
        message.AddByte((byte)GameOutgoingPacketType.CreatureOutfit);

        message.AddUInt32(Creature.CreatureId);
        message.AddUInt16(Creature.Outfit.LookType);

        if (Creature.Outfit.LookType > 0)
        {
            message.AddByte(Creature.Outfit.Head);
            message.AddByte(Creature.Outfit.Body);
            message.AddByte(Creature.Outfit.Legs);
            message.AddByte(Creature.Outfit.Feet);
            message.AddByte(Creature.Outfit.Addon);
            
            return;
        }

        message.AddUInt16(Creature.Outfit.LookItemTypeId);
    }
}