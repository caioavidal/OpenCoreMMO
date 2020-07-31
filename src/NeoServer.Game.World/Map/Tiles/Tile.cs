using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Contracts.World.Tiles;
using NeoServer.Game.Enums;
using NeoServer.Game.Enums.Location;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace NeoServer.Game.World.Map.Tiles
{
    public class Tile : IWalkableTile
    {
        public Tile(Coordinate coordinate, TileFlag tileFlag, IGround ground, IItem[] topItems, IItem[] items)
        {
            Location = new Location(coordinate.X, coordinate.Y, coordinate.Z);
            flags |= (byte)tileFlag;
            AddContent(ground, topItems, items);
        }

        private byte[] cache;

        public Location Location { get; }

        public ushort StepSpeed { get; private set; }

        public FloorChangeDirection FloorDirection { get; private set; }
        public byte MovementPenalty { get; private set; }

        public ushort Ground { get; private set; }
        //public ConcurrentStack<IItem> TopItems { get; private set; } = new ConcurrentStack<IItem>();
        public ushort[] TopItems { get; private set; }

        public ConcurrentStack<IItem> DownItems { get; private set; } = new ConcurrentStack<IItem>();
        public ConcurrentStack<ICreature> Creatures { get; private set; } = new ConcurrentStack<ICreature>();

        public bool CannotLogout => HasFlag(TileFlags.NoLogout);
        public bool ProtectionZone => HasFlag(TileFlags.ProtectionZone);


        public byte NextStackPosition
        {
            get
            {
                var n = 0;

                foreach (var item in TopItems)
                {
                    n++;
                }
                return (byte)++n;
            }
        }
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

            foreach (var clientId in TopItems)
            {
                ++n;
                if (id == clientId)
                {
                    return (byte)n;
                }
            }

            foreach (var item in DownItems)
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

            if (Ground != default && thing is IGround)
            {
                return 0;
            }

            var n = 0;

            foreach (var clientId in TopItems)
            {
                ++n;
                if (thing is IItem item && item.ClientId == clientId)
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

        private TileOperationResult AddThingToTile(Contracts.Items.IThing thing)
        {
            var operations = new TileOperationResult();

            if (thing is ICreature creature)
            {
                Creatures.Push(creature);
                creature.Tile = this;
            }
            else if (thing is IItem item)
            {

                IItem topStackItem;
                if (!DownItems.TryPeek(out topStackItem))
                {
                    DownItems.Push(item);
                    operations.Add(Operation.Added);

                }
                else if (item is ICumulativeItem cumulative &&
                    topStackItem is ICumulativeItem topCumulative &&
                    topStackItem.ClientId == cumulative.ClientId &&
                    topCumulative.TryJoin(ref cumulative))
                {
                    operations.Add(Operation.Updated);

                    if (cumulative != null)
                    {
                        DownItems.Push(cumulative);
                        operations.Add(Operation.Added);

                    }
                }
                else
                {
                    DownItems.Push(item);
                    operations.Add(Operation.Added);

                }

            }
            SetCacheAsExpired();
            return operations;
        }
        public Result<TileOperationResult> AddThing(ref IMoveableThing thing)
        {
            var operations = AddThingToTile(thing);
            thing.SetNewLocation(Location);
            return new Result<TileOperationResult>(operations);
        }

        /// <summary>
        /// Get the top item on DownItems's stack
        /// </summary>
        public IItem TopItemOnStack => DownItems.TryPeek(out IItem item) ? item : null;

        private byte flags;

        private bool HasFlag(TileFlags flag) => ((uint)flag & flags) != 0;

        private void AddContent(IGround ground, IItem[] topItems, IItem[] items)
        {
            TopItems = new ushort[topItems.Length];

            if (ground != null)
            {
                StepSpeed = ground.StepSpeed;
                MovementPenalty = ground.MovementPenalty;
                Ground = ground.ClientId;
            }

            for (int i = 0; i < topItems.Length; i++)
            {
                if (topItems[i].FloorDirection != default)
                {
                    FloorDirection = topItems[i].FloorDirection;
                }

                FloorDirection = topItems[i].FloorDirection;
                TopItems[i] = topItems[i].ClientId;
            }

            foreach (var item in items)
            {
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
            //else if (itemToRemove.IsAlwaysOnTop)
            //{
            //    TopItems.TryPop(out var item);
            //    removedThing = item;

            //}
            else
            {
                IItem topStackItem;
                if (DownItems.TryPeek(out topStackItem))
                {
                    if (thing is ICumulativeItem cumulative &&
                   topStackItem is ICumulativeItem topCumulative)
                    {
                        topCumulative.Reduce(amount);

                        if (topCumulative.Amount == 0)
                        {
                            DownItems.TryPop(out var item);
                        }
                        removedThing = topCumulative.Clone(amount);

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

            foreach (var clientId in TopItems)
            {
                if (countThings == 9)
                {
                    break;
                }
                var raw = BitConverter.GetBytes(clientId);
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
