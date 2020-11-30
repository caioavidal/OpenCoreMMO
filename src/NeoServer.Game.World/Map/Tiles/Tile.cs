using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Contracts.World.Tiles;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace NeoServer.Game.World.Map.Tiles
{
    public class Tile : BaseTile, IDynamicTile
    {
        public event AddThingToTileDel OnThingAddedToTile;

        public Tile(Coordinate coordinate, TileFlag tileFlag, IGround ground, IItem[] topItems, IItem[] items)
        {
            Location = new Location((ushort)coordinate.X, (ushort)coordinate.Y, (byte)coordinate.Z);
            flags |= (byte)tileFlag;
            AddContent(ground, topItems, items);
        }

        private byte[] cache;

        public Location Location { get; }
        public ushort StepSpeed => Ground.StepSpeed;


        public FloorChangeDirection FloorDirection { get; private set; }
        public byte MovementPenalty => Ground.MovementPenalty;

        public IGround Ground { get; private set; }
        public Stack<IItem> TopItems { get; private set; }

        public Stack<IItem> DownItems { get; private set; }
        public Dictionary<uint, IWalkableCreature> Creatures { get; private set; } = new Dictionary<uint, IWalkableCreature>();
        public bool CannotLogout => HasFlag(TileFlags.NoLogout);
        public bool ProtectionZone => HasFlag(TileFlags.ProtectionZone);

        public bool HasCreature => Creatures.Count > 0;
        /// <summary>
        /// Get the top item on DownItems's stack
        /// </summary>
        public override IItem TopItemOnStack => DownItems != null && DownItems.TryPeek(out IItem item) ? item : TopItems is not null && TopItems.TryPeek(out item) ? item : Ground;

        public IMagicField MagicField
        {
            get
            {
                if (!HasFlag(TileFlags.MagicField))
                {
                    return null;
                }

                foreach (var downItem in DownItems)
                {
                    if (downItem is IMagicField magicField)
                    {
                        return magicField;
                    }
                }
                return null;
            }
        }

        public ICreature GetTopVisibleCreature(ICreature creature)
        {
            foreach (var tileCreature in Creatures.Values)
            {
                //var tileCreature = Creatures[creatureId];
                if (creature != null)
                {
                    if (creature.CanSee(tileCreature))
                    {
                        return tileCreature;
                    }
                }
                else
                {
                    var isPlayer = tileCreature is IPlayer;

                    var player = isPlayer ? tileCreature as IPlayer : null;

                    if (!tileCreature.IsInvisible)
                    {
                        if (!isPlayer || !player.IsInvisible)
                        {
                            return tileCreature;
                        }
                    }
                }
            }

            return null;
        }

        public override bool TryGetStackPositionOfThing(IPlayer observer, IThing thing, out byte stackPosition)
        {
            stackPosition = default;

            if (thing is IItem item)
            {
                return TryGetStackPositionOfItem(observer, item, out stackPosition);
            }
            else if (thing is ICreature creature)
            {
                return TryGetStackPositionOfCreature(observer, creature, out stackPosition);
            }

            return false;
        }
        private bool TryGetStackPositionOfItem(IPlayer observer, IItem item, out byte stackPosition)
        {
            stackPosition = 0;

            var id = item.ClientId;
            if (id == default)
            {
                throw new ArgumentNullException(nameof(id));
            }


            if (Ground.ClientId == id)
            {
                return true;
            }
            if (Ground.ClientId != 0)
            {
                ++stackPosition;
            }

            if (item.IsAlwaysOnTop)
            {
                foreach (var topItem in TopItems)
                {
                    if (id == topItem.ClientId)
                    {
                        return true;
                    }
                    else if (++stackPosition == 10)
                    {
                        return false;
                    }
                }
            }
            else
            {

                stackPosition += (byte)(TopItems?.Count ?? 0);
                if (stackPosition >= 10)
                {
                    return false;
                }
            }

            foreach (var creature in Creatures)
            {
                if (observer.CanSee(creature.Value))
                {
                    if (++stackPosition >= 10)
                    {
                        return false;
                    }
                }
            }

            if (!item.IsAlwaysOnTop && DownItems is not null)
            {
                foreach (var downItem in DownItems)
                {
                    if (id == downItem.ClientId)
                    {
                        return true;
                    }
                    else if (++stackPosition >= 10)
                    {
                        return false;
                    }
                }
            }

            return false;
        }

        private bool TryGetStackPositionOfCreature(IPlayer observer, ICreature creature, out byte stackPosition)
        {
            stackPosition = default;

            var id = creature.CreatureId;
            if (id == default)
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (Ground.ClientId != 0)
            {
                stackPosition++;
            }

            if (TopItems != null)
            {
                stackPosition += (byte)TopItems.Count;
                if (stackPosition >= 10)
                {
                    return false;
                }
            }

            foreach (var c in Creatures.Values?.Reverse())
            {
                if (c == creature)
                {
                    return true;
                }
                else if (observer.CanSee(creature))
                {
                    if (++stackPosition >= 10)
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        public uint GetThingByStackPosition(byte stackPosition)
        {
            if (stackPosition == 0)
            {
                return Ground.ClientId ;
            }
            var n = 0;

            if (TopItems != null)
            {
                foreach (var item in TopItems)
                {
                    ++n;

                    if (n == stackPosition)
                    {
                        return item.ClientId;
                    }
                }
            }

            foreach (var creature in Creatures)
            {
                ++n;

                if (n == stackPosition)
                {
                    return creature.Key;
                }
            }

            if (DownItems != null)
            {
                foreach (var item in DownItems)
                {
                    ++n;
                    if (n == stackPosition)
                    {
                        return item.ClientId;
                    }
                }
            }

            // return byte.MaxValue; // TODO: throw?
            throw new Exception("stackposition invalid");
        }

        public bool HasBlockPathFinding
        {
            get
            {
                if (DownItems is null) return false;

                foreach (var item in DownItems)
                {
                    if (item.BlockPathFinding)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        private ITileOperationResult AddThingToTile(IThing thing)
        {
            var operations = new TileOperationResult();

            if (thing is IWalkableCreature creature)
            {
                if (HasCreature)
                {
                    return operations;
                }
                Creatures.TryAdd(creature.CreatureId, creature);
                operations.Add(Operation.Added, creature);
                creature.Tile = this;
            }
            else if (thing is IItem item)
            {
                IItem topStackItem;

                if (item.IsAlwaysOnTop)
                {
                    if (TopItems is null) TopItems = new Stack<IItem>(10);

                    if (TopItems.TryPeek(out var topItem) && topItem.ClientId == item.ClientId)
                    {
                        operations.Add(Operation.Added, item);
                    }
                    else
                    {
                        TopItems.Push(item);
                        operations.Add(Operation.Added, item);
                    }
                }
                else
                {
                    if (DownItems is null) DownItems = new Stack<IItem>(10);

                    if (!DownItems.TryPeek(out topStackItem))
                    {
                        DownItems.Push(item);
                        operations.Add(Operation.Added, item);

                    }
                    else if (item is ICumulativeItem cumulative &&
                        topStackItem is ICumulativeItem topCumulative &&
                        topStackItem.ClientId == cumulative.ClientId &&
                        topCumulative.TryJoin(ref cumulative))
                    {
                        operations.Add(Operation.Updated, topCumulative);

                        if (cumulative is not null)
                        {
                            DownItems.Push(cumulative);
                            operations.Add(Operation.Added, cumulative);
                        }
                    }
                    else
                    {
                        DownItems.Push(item);
                        operations.Add(Operation.Added, item);

                    }
                }
            }

            SetCacheAsExpired();
            return operations;
        }

        public Result<ITileOperationResult> AddThing(IThing thing)
        {
            var operations = AddThingToTile(thing);
            if(operations.HasAnyOperation) thing.Location = Location;
            return new Result<ITileOperationResult>(operations);
        }



        private byte flags;
        private bool HasFlag(TileFlags flag) => ((uint)flag & flags) != 0;

        private void AddContent(IGround ground, IItem[] topItems, IItem[] items)
        {
            if (topItems.Length > 0)
            {
                TopItems = new Stack<IItem>(10);
            }
            if (items.Length > 0)
            {
                DownItems = new Stack<IItem>(10);
            }

            if (ground != null)
            {
                Ground = ground;
                FloorDirection = ground.Metadata.Attributes.GetFloorChangeDirection();
            }

            foreach (var item in topItems)
            {
                if (item.FloorDirection != default)
                {
                    FloorDirection = item.FloorDirection;
                }

                FloorDirection = item.FloorDirection;
                TopItems.Push(item);
            }

            foreach (var item in items)
            {
                AddThing(item);
            }
        }
        public Result<ITileOperationResult> RemoveThing(IThing thing, byte amount, out IThing removedThing)
        {
            amount = amount == 0 ? 1 : amount;
            var operations = new TileOperationResult();

            removedThing = null;
            var itemToRemove = thing as IItem;

            if (thing is ICreature c)
            {
                Creatures.Remove(c.CreatureId, out var creature);
                operations.Add(Operation.Removed, creature);
                removedThing = creature;
            }
            else if (itemToRemove.IsAlwaysOnTop)
            {
                TopItems.TryPop(out var item);
                operations.Add(Operation.Removed, item);
                removedThing = item;
            }
            else
            {
                if (DownItems.TryPeek(out var topStackItem))
                {
                    if (thing is ICumulativeItem && topStackItem is ICumulativeItem topCumulative)
                    {
                        topCumulative.Reduce(amount);

                        if (topCumulative.Amount == 0)
                        {
                            DownItems.TryPop(out var item);
                            operations.Add(Operation.Removed, item);
                        }
                        else
                        {
                            operations.Add(Operation.Updated, topCumulative);
                        }
                        removedThing = topCumulative.Clone(amount);
                    }
                    else
                    {
                        DownItems.TryPop(out var item);
                        operations.Add(Operation.Removed, item);
                        removedThing = item;
                    }
                }
            }

            SetCacheAsExpired();
            return new Result<ITileOperationResult>(operations);
        }
        private void SetCacheAsExpired() => cache = null;

        public byte[] GetRaw(IPlayer playerRequesting)
        {
            if (cache != null && !Creatures.Any())
            {
                return cache;
            }

            Span<byte> stream = stackalloc byte[930]; //max possible length

            int countThings = 0;
            int countBytes = 0;

            if (Ground != default)
            {
                BitConverter.GetBytes(Ground.ClientId).AsSpan().CopyTo(stream);

                countThings++;
                countBytes += 2;
            }

            if (TopItems != null)
            {
                foreach (var item in TopItems.Reverse()) //todo: remove reverse
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
            }

            foreach (var creature in Creatures)
            {
                if (countThings == 9)
                {
                    break;
                }

                var raw = creature.Value.GetRaw(playerRequesting);
                raw.CopyTo(stream.Slice(countBytes, raw.Length));

                countThings++;
                countBytes += raw.Length;
            }

            if (DownItems != null)
            {
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
            }

            cache = stream.Slice(0, countBytes).ToArray();
            return cache;
        }
    }
}
