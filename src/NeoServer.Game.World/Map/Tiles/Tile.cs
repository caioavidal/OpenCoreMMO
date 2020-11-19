using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Contracts.World.Tiles;
using NeoServer.Game.Enums;
using NeoServer.Game.Enums.Location;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace NeoServer.Game.World.Map.Tiles
{
    public class Tile : BaseTile, IWalkableTile
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
        private int CreaturesCount;
        public ushort StepSpeed { get; private set; }

        public FloorChangeDirection FloorDirection { get; private set; }
        public byte MovementPenalty { get; private set; }

        public ushort Ground { get; private set; }
        //public ConcurrentStack<IItem> TopItems { get; private set; } = new ConcurrentStack<IItem>();
        public ushort[] TopItems { get; private set; }

        public ConcurrentStack<IItem> DownItems { get; private set; } = new ConcurrentStack<IItem>();
        public ConcurrentDictionary<uint, IWalkableCreature> Creatures { get; private set; } = new ConcurrentDictionary<uint, IWalkableCreature>();
        private List<uint> creatures = new List<uint>();

        public bool CannotLogout => HasFlag(TileFlags.NoLogout);
        public bool ProtectionZone => HasFlag(TileFlags.ProtectionZone);

        public bool HasCreature => CreaturesCount > 0;

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
            foreach (var creatureId in creatures)
            {
                var tileCreature = Creatures[creatureId];
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

        public override bool TryGetStackPositionOfThing(IPlayer player, IThing thing, out byte stackPosition)
        {
            stackPosition = default;

            if (thing is IItem item)
            {
                return TryGetStackPositionOfItem(player, item, out stackPosition);
            }
            else if (thing is ICreature creature)
            {
                return TryGetStackPositionOfCreature(player, creature, out stackPosition);
            }

            return false;
        }
        public bool TryGetStackPositionOfItem(IPlayer player, IItem item, out byte stackPosition)
        {
            stackPosition = default;

            var id = item.ClientId;
            if (id == default)
            {
                throw new ArgumentNullException(nameof(id));
            }

            byte n = 0;

            if (Ground == id)
            {
                stackPosition = 0;
                return true;
            }
            if (Ground != 0)
            {
                ++n;
            }

            if (item.IsAlwaysOnTop)
            {
                foreach (var clientId in TopItems)
                {
                    if (id == clientId)
                    {
                        stackPosition = n;
                        return true;
                    }
                    else if (++n == 10)
                    {
                        return false;
                    }
                }

            }
            else
            {
                n += (byte)TopItems.Length;
                if (n >= 10)
                {
                    return false;
                }
            }

            foreach (var creature in Creatures)
            {
                if (player.CanSee(creature.Value))
                {
                    if (++n >= 10)
                    {
                        return false;
                    }
                }
            }

            if (!item.IsAlwaysOnTop)
            {
                foreach (var downItem in DownItems)
                {
                    if (id == downItem.ClientId)
                    {
                        stackPosition = n;
                        return true;
                    }
                    else if (++n >= 10)
                    {
                        return false;
                    }
                }
            }

            return false;
            //throw new Exception("Thing not found in tile.");
        }

        public bool TryGetStackPositionOfCreature(IPlayer player, ICreature creature, out byte stackPosition)
        {
            stackPosition = default;

            var id = creature.CreatureId;
            if (id == default)
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (Ground != 0)
            {
                stackPosition++;
            }


            foreach (var clientId in TopItems)
            {

                stackPosition += (byte)TopItems.Length;
                if (stackPosition >= 10)
                {
                    return false;
                }
            }

            foreach (var c in Creatures.Values?.Reverse())
            {
                if (c.CreatureId == creature.CreatureId)
                {
                    return true;
                }
                else if (player.CanSee(creature))
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
                return Ground;
            }

            var n = 0;

            foreach (var clientId in TopItems)
            {
                ++n;

                if (n == stackPosition)
                {
                    return clientId;
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

            foreach (var item in DownItems)
            {
                ++n;
                if (n == stackPosition)
                {
                    return item.ClientId;
                }
            }

            // return byte.MaxValue; // TODO: throw?
            throw new Exception("stackposition invalid");
        }


        public bool HasBlockPathFinding
        {
            get
            {
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

        private TileOperationResult AddThingToTile(IThing thing)
        {
            var operations = new TileOperationResult();

            if (thing is IWalkableCreature creature)
            {
                if (HasCreature)
                {
                    operations.Add(Operation.None);
                    return operations;
                }
                if(Creatures.TryAdd(creature.CreatureId, creature))
                {
                    creatures.Add(creature.CreatureId);
                    CreaturesCount++;
                }
                
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
            if (operations.Operations != null)
            {
                foreach (var operation in operations.Operations)
                {
                    switch (operation)
                    {
                        case Operation.Added: OnThingAddedToTile?.Invoke(thing, this); break;
                        //case Operation.Updated: OnThingUpdatedOnTile?.Invoke(tile.TopItemOnStack, tile, stackPosition); break;
                        default: break;
                    }
                }
            }

            SetCacheAsExpired();
            return operations;
        }

        public Result<TileOperationResult> AddThing(ref IThing thing)
        {
            var operations = AddThingToTile(thing);

            return new Result<TileOperationResult>(operations);
        }
        public Result<TileOperationResult> AddThing(ref IMoveableThing thing)
        {
            var operations = AddThingToTile(thing);

            if (!operations.HasNoneOperation)
            {
                thing.SetNewLocation(Location);
            }

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

            if (thing is ICreature c)
            {

                if(Creatures.TryRemove(c.CreatureId, out var creature))
                {
                    creatures.Remove(c.CreatureId);
                    CreaturesCount--;
                }
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

            if (cache != null && !Creatures.Any())
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

                var raw = creature.Value.GetRaw(playerRequesting);
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
