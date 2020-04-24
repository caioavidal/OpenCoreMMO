using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Contracts.World.Tiles;
using NeoServer.Game.Enums.Location;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace NeoServer.Game.World.Map.Tiles
{
    public class Tile : IWalkableTile
    {
        public Tile(Coordinate coordinate, TileFlag tileFlag, IItem[] items)
        {
            Location = new Location(coordinate.X, coordinate.Y, coordinate.Z);
            flags |= (byte)tileFlag;
            AddContent(items);
        }

        private byte[] cache;

        public Location Location { get; }

        public ushort StepSpeed { get; private set; }

        public FloorChangeDirection FloorDirection { get; private set; }
        public byte MovementPenalty { get; private set; }

        public ushort Ground { get; private set; }
        public ConcurrentStack<IItem> Top1 { get; private set; } = new ConcurrentStack<IItem>();
        public ConcurrentStack<IItem> Top2 { get; private set; } = new ConcurrentStack<IItem>();
        public ConcurrentStack<IItem> DownItems { get; private set; } = new ConcurrentStack<IItem>();
        public ConcurrentStack<ICreature> Creatures { get; private set; } = new ConcurrentStack<ICreature>();

        public bool CannotLogout => HasFlag(TileFlags.NoLogout);
        public bool ProtectionZone => HasFlag(TileFlags.ProtectionZone);

        public byte GetStackPositionOfThing(IThing thing)
        {
            if (thing == null)
            {
                throw new ArgumentNullException(nameof(thing));
            }

            if (Ground != default && thing is IGroundItem)
            {
                return 0;
            }

            var n = 0;

            foreach (var item in Top1)
            {
                ++n;
                if (thing == item)
                {
                    return (byte)n;
                }
            }

            foreach (var item in Top2)
            {
                ++n;
                if (thing == item)
                {
                    return (byte)n;
                }
            }

            foreach (var creature in Creatures)
            {
                ++n;

                if (thing is ICreature thisCreature && thisCreature == creature)
                {
                    return (byte)n;
                }
            }

            foreach (var item in DownItems)
            {
                ++n;
                if (thing == item)
                {
                    return (byte)n;
                }
            }

            // return byte.MaxValue; // TODO: throw?
            throw new Exception("Thing not found in tile.");
        }

        private void AddThingToTile(Contracts.Items.IThing thing)
        {
            var thingType = new ThingType(thing);

            if (thing is ICreature creature)
            {
                Creatures.Push(creature);
                creature.Tile = this;
            }
            else if (thing is IItem item)
            {
                if (thingType.IsGround)
                {
                    var ground = item as IGroundItem;
                    StepSpeed = ground.StepSpeed;
                    MovementPenalty = ground.MovementPenalty;
                    Ground = item.ClientId;
                }
                else if (thingType.IsTop1)
                {
                    Top1.Push(item);
                }
                else if (thingType.IsTop2)
                {
                    Top2.Push(item);
                }
                else
                {
                    IItem topStackItem;
                    if (!DownItems.TryPeek(out topStackItem))
                    {
                        DownItems.Push(item);
                    }
                    else if (item is ICumulativeItem cumulative &&
                        topStackItem is ICumulativeItem topCumulative &&
                        topCumulative.TryJoin(ref cumulative))
                    {

                        if (cumulative != null)
                        {
                            DownItems.Push(cumulative);
                        }
                    }
                    else
                    {
                        DownItems.Push(item);
                    }
                }
            }
            SetCacheAsExpired();
        }
        public void AddThing(ref IMoveableThing thing)
        {
            AddThingToTile(thing);
            thing.SetNewLocation(Location);
        }

        private byte flags;

        private bool HasFlag(TileFlags flag) => ((uint)flag & flags) != 0;

        private void AddContent(IItem[] items)
        {
            foreach (var item in items)
            {
                if (item.FloorDirection != default)
                {
                    FloorDirection = item.FloorDirection;
                }
                AddThingToTile(item);
            }
        }
        public void RemoveThing(ref IMoveableThing thing)
        {
            var thingType = new ThingType(thing);

            if (thingType.IsCreature)
            {
                Creatures.TryPop(out var result);
            }

            else if (thingType.IsTop1)
            {
                Top1.TryPop(out var item);

            }
            else if (thingType.IsTop2)
            {
                Top2.TryPop(out var item);

            }
            else
            {
                DownItems.TryPop(out var item);
            }
            SetCacheAsExpired();
        }

        private void SetCacheAsExpired() => cache = null;

        public byte[] GetRaw(IPlayer playerRequesting)
        {
            if (Location == new Location(164, 459, 7))
            {

            }

            if (cache != null)
            {
                return cache;
            }

            Span<byte> stream = stackalloc byte[930]; //max possible length

            int countThings = 0;
            int countBytes = 0;

            if (Ground != default)
            {
                BitConverter.GetBytes(Ground).AsSpan().CopyTo(stream);

                countThings++;
                countBytes += 2;
            }

            foreach (var item in Top1.Reverse().Concat(Top2.Reverse()))
            {
                if (countThings == 9)
                {
                    break;
                }
                var raw = item.GetRaw();
                raw.CopyTo(stream.Slice(countBytes, raw.Length));

                countThings++;
                countBytes += raw.Length;
            }

            foreach (var creature in Creatures)
            {
                if (countThings == 9)
                {
                    break;
                }

                var raw = creature.GetRaw(playerRequesting);
                raw.CopyTo(stream.Slice(countBytes, raw.Length));

                countThings++;
                countBytes += raw.Length;
            }

            foreach (var item in DownItems)
            {
                if (countThings == 9)
                {
                    break;
                }

                var raw = item.GetRaw();
                raw.CopyTo(stream.Slice(countBytes, raw.Length));

                countThings++;
                countBytes += raw.Length;
            }

            cache = stream.Slice(0, countBytes).ToArray();
            return cache;


        }

    }

    public readonly ref struct ThingType
    {
        public readonly bool IsGround;
        public readonly bool IsTop1;
        public readonly bool IsTop2;
        public readonly bool IsCreature;
        public ThingType(Contracts.Items.IThing thing)
        {
            IsGround = thing is IGroundItem;
            IsTop1 = thing is IItem top1 && top1.IsAlwaysOnTop;
            IsTop2 = thing is IItem item && item.IsBottom;
            IsCreature = thing is ICreature;
        }
    }
}
