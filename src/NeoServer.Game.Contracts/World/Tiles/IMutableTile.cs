using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Enums;
using NeoServer.Game.Enums.Location;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Server.Model.Players.Contracts;
using System.Collections.Concurrent;

namespace NeoServer.Game.Contracts.World.Tiles
{
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
        byte NextStackPosition { get; }
        IItem TopItemOnStack { get; }
        bool HasCreature { get; }

        Result<TileOperationResult> AddThing(ref IMoveableThing thing);
        byte[] GetRaw(IPlayer playerRequesting = null);
        byte GetStackPositionOfItem(ushort id);
        byte GetStackPositionOfThing(Items.IThing thing);
        Result<TileOperationResult> MoveThing(ref IMoveableThing thing, IWalkableTile dest, byte amount = 1);
        IThing RemoveThing(ref IMoveableThing thing, byte count = 1);
    }
}
