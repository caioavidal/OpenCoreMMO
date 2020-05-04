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
        public ConcurrentStack<IItem> TopItems { get; private set; } = new ConcurrentStack<IItem>();
        public ConcurrentStack<IItem> DownItems { get; private set; } = new ConcurrentStack<IItem>();
        public ConcurrentStack<ICreature> Creatures { get; private set; } = new ConcurrentStack<ICreature>();

        public bool CannotLogout => HasFlag(TileFlags.NoLogout);
        public bool ProtectionZone => HasFlag(TileFlags.ProtectionZone);


        public byte GetStackPositionOfItem(ushort id)
        {
            if (id == default)
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (Ground == id)
            {
                return 0;
            }

            var n = 0;

            foreach (var item in TopItems.Concat(DownItems))
            {
                ++n;
                if (id == item.ClientId)
                {
                    return (byte)n;
                }
            }
            throw new Exception("Thing not found in tile.");
        }
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

            foreach (var item in TopItems)
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
            
            if (thing is ICreature creature)
            {
                Creatures.Push(creature);
                creature.Tile = this;
            }
            else if (thing is IItem item)
            {
                if (thing is IGroundItem)
                {
                    var ground = item as IGroundItem;
                    StepSpeed = ground.StepSpeed;
                    MovementPenalty = ground.MovementPenalty;
                    Ground = item.ClientId;
                }
                else if (item.IsAlwaysOnTop)
                {
                    TopItems.Push(item);
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
                        topStackItem.ClientId == cumulative.ClientId &&
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

      
        public IThing RemoveThing(ref IMoveableThing thing, byte amount = 1)
        {

            IThing removedThing = null;
            var itemToRemove = thing as IItem;

            if (thing is ICreature)
            {
                Creatures.TryPop(out var creature);
                removedThing = creature;
            }
            else if (itemToRemove.IsAlwaysOnTop)
            {
                TopItems.TryPop(out var item);
                removedThing = item;

            }
            else
            {
                IItem topStackItem;
                if (DownItems.TryPeek(out topStackItem))
                {
                    if (thing is ICumulativeItem cumulative &&
                   topStackItem is ICumulativeItem topCumulative)
                    {
                        var splitedItem = topCumulative.Split(amount);

                        if (topCumulative.Amount == 0)
                        {
                            DownItems.TryPop(out var item);
                        }
                        removedThing = splitedItem;

                    }
                    else
                    {
                        DownItems.TryPop(out var item);
                        removedThing = item;
                    }
                }
            }
            SetCacheAsExpired();
            return removedThing;
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

            foreach (var item in TopItems.Reverse())
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

   
}
