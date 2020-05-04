using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Enums.Location;
using NeoServer.Server.Model.Players.Contracts;
using System.Collections.Concurrent;

namespace NeoServer.Game.Contracts.World.Tiles
{
    public interface IWalkableTile : ITile
    {
        ushort Ground { get; }
        ConcurrentStack<IItem> TopItems { get; }
        ConcurrentStack<IItem> DownItems { get; }
        ConcurrentStack<ICreature> Creatures { get; }
        ushort StepSpeed { get; }
        bool CannotLogout { get; }
        bool ProtectionZone { get; }
        FloorChangeDirection FloorDirection { get; }
        byte MovementPenalty { get; }

        void AddThing(ref IMoveableThing thing);
        byte[] GetRaw(IPlayer playerRequesting = null);
        byte GetStackPositionOfItem(ushort id);
        byte GetStackPositionOfThing(Items.IThing thing);
        IThing RemoveThing(ref IMoveableThing thing, byte count = 1);
    }
}
