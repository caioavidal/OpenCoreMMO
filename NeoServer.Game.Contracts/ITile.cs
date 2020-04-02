using System.Collections.Generic;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Item;
using NeoServer.Game.Enums.Location;
using NeoServer.Game.Enums.Location.Structs;

namespace NeoServer.Game.Contracts
{
    public interface ITile
    {
        bool HandlesCollision { get; }

        IEnumerable<IItem> ItemsWithCollision { get; }

        bool HandlesSeparation { get; }

        IEnumerable<IItem> ItemsWithSeparation { get; }

        Location Location { get; }

        byte Flags { get; }

        bool IsHouse { get; }

        bool BlocksThrow { get; }

        bool BlocksPass { get; }

        bool BlocksLay { get; }

        IItem Ground { get; }

        IEnumerable<uint> CreatureIds { get; }

        IEnumerable<IItem> TopItems1 { get; }

        IEnumerable<IItem> TopItems2 { get; }

        IEnumerable<IItem> DownItems { get; }

        byte[] CachedDescription { get; }

        void AddThing(ref IThing thing, byte count = 1);

        void RemoveThing(ref IThing thing, byte count = 1);

        IThing GetThingAtStackPosition(byte stackPosition);

        byte GetStackPositionOfThing(IThing thing);

        void SetFlag(TileFlag flag);

        bool HasThing(IThing thing, byte count = 1);

        IItem BruteFindItemWithId(ushort typeId);

        IItem BruteRemoveItemWithId(ushort id);

        bool CanBeWalked(byte avoidDamageType = 0);
        void RemoveCreature(ICreature c);
        void AddCreature(ICreature creature);
    }
}
