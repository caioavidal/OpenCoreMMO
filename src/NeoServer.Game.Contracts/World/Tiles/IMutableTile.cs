using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Enums;
using NeoServer.Game.Enums.Location;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Server.Model.Players.Contracts;
using System.Collections.Concurrent;

namespace NeoServer.Game.Contracts.World.Tiles
{
    public delegate void AddThingToTileDel(IThing thing, ITile tile);

    public interface IWalkableTile : ITile
    {
        ushort Ground { get; }
        ushort[] TopItems { get; }
        ConcurrentStack<IItem> DownItems { get; }
        ConcurrentDictionary<uint, ICreature> Creatures { get; }
        ushort StepSpeed { get; }
        bool CannotLogout { get; }
        bool ProtectionZone { get; }
        FloorChangeDirection FloorDirection { get; }
        byte MovementPenalty { get; }
        IItem TopItemOnStack { get; }
        bool HasCreature { get; }
        IMagicField MagicField { get; }

        event AddThingToTileDel OnThingAddedToTile;

        Result<TileOperationResult> AddThing(ref IMoveableThing thing);
        byte[] GetRaw(IPlayer playerRequesting = null);
        uint GetThingByStackPosition(byte stackPosition);
        ICreature GetTopVisibleCreature(ICreature creature);
        bool HasBlockPathFinding { get; }
        //Result<TileOperationResult> MoveThing(ref IMoveableThing thing, IWalkableTile dest, byte amount = 1);
        IThing RemoveThing(ref IMoveableThing thing, byte count = 1);
        Result<TileOperationResult> AddThing(ref IThing thing);
    }
}
