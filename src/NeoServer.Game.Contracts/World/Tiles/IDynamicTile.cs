using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Server.Model.Players.Contracts;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace NeoServer.Game.Contracts.World.Tiles
{
    public delegate void AddThingToTileDel(IThing thing, ITile tile);

    public interface IDynamicTile : ITile
    {
        ushort Ground { get; }
        Stack<IItem> TopItems { get; }
        Stack<IItem> DownItems { get; }
        Dictionary<uint, IWalkableCreature> Creatures { get; }
        ushort StepSpeed { get; }
        bool CannotLogout { get; }
        bool ProtectionZone { get; }
        FloorChangeDirection FloorDirection { get; }
        byte MovementPenalty { get; }
        IItem TopItemOnStack { get; }
        bool HasCreature { get; }
        IMagicField MagicField { get; }

        event AddThingToTileDel OnThingAddedToTile;

        byte[] GetRaw(IPlayer playerRequesting = null);
        uint GetThingByStackPosition(byte stackPosition);
        ICreature GetTopVisibleCreature(ICreature creature);
        bool HasBlockPathFinding { get; }
        Result<ITileOperationResult> RemoveThing(IThing thing, byte count, out IThing removedThing);
        Result<ITileOperationResult> AddThing(IThing thing);
    }
}
