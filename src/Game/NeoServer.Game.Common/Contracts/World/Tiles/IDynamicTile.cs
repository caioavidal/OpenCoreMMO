using System.Collections.Generic;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Location;

namespace NeoServer.Game.Common.Contracts.World.Tiles;

public delegate void AddCreatureToTile(ICreature creature, ITile tile);

public interface IDynamicTile : ITile, IHasItem
{
    IGround Ground { get; }
    List<IWalkableCreature> Creatures { get; }
    ushort StepSpeed { get; }
    bool ProtectionZone { get; }
    FloorChangeDirection FloorDirection { get; }
    bool HasCreature { get; }
    IMagicField MagicField { get; }

    bool HasBlockPathFinding { get; }
    bool HasHole { get; }
    List<IPlayer> Players { get; }
    bool HasTeleport(out ITeleport teleport);

    byte[] GetRaw(IPlayer playerRequesting = null);
    ICreature GetTopVisibleCreature(ICreature creature);
    bool TryGetStackPositionOfItem(IItem item, out byte stackPosition);
    event AddCreatureToTile CreatureAdded;
    IItem[] RemoveAllItems();
    ICreature[] RemoveAllCreatures();
    bool RemoveTopItem(out IItem removedItem);
    bool HasCreatureOfType<T>() where T:ICreature;
}