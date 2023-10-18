using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Item.Items;

public class Ground : Bases.Item, IGround
{
    public Ground(IItemType type, Location location) : base(type, location)
    {
    }

    public event CreatureWalkedThroughGround OnCreatureWalkedThrough;
    public ushort StepSpeed => (Metadata?.Speed ?? 0) != 0 ? Metadata.Speed : (ushort)150;
    public byte MovementPenalty => Metadata.Attributes.GetAttribute<byte>(ItemAttribute.Waypoints);

    public void CreatureEntered(ICreature creature)
    {
        OnCreatureWalkedThrough?.Invoke(creature, this);
    }

    public static bool IsApplicable(IItemType type)
    {
        return type.Group is ItemGroup.Ground;
    }
}