using System;
using System.Collections.Generic;
using System.Linq;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.World.Map;
using NeoServer.Game.World.Map.Tiles;

namespace NeoServer.Game.World.Models.Tiles
{
    public class DynamicTile : BaseTile, IDynamicTile
    {
        private byte[] _cache;
        
        public DynamicTile(Coordinate coordinate, TileFlag tileFlag, IGround ground, IItem[] topItems, IItem[] items)
        {
            Location = new Location((ushort)coordinate.X, (ushort)coordinate.Y, (byte)coordinate.Z);
            Flags |= (byte)tileFlag;
            AddContent(ground, topItems, items);
        }

        public int ItemsCount => (DownItems?.Count ?? 0) + (TopItems?.Count ?? 0) + (Ground is null ? 0 : 1);
        public override int ThingsCount =>  Creatures?.Count ?? 0 + ItemsCount; 

        public ushort StepSpeed => Ground.StepSpeed;

        public override ICreature TopCreatureOnStack => Creatures?.FirstOrDefault().Value;

        public bool HasHole =>
            Ground is { } &&
            Ground.Metadata.Attributes.TryGetAttribute(ItemAttribute.FloorChange, out var floorChange) &&
            floorChange == "down";

        public byte MovementPenalty => Ground.MovementPenalty;

        public IGround Ground { get; private set; }
        public Stack<IItem> TopItems { get; private set; }
        public Stack<IItem> DownItems { get; private set; }
        public Dictionary<uint, IWalkableCreature> Creatures { get; private set; }

        public IItem[] AllItems
        {
            get
            {
                var currentIndex = 0;
                var items = new IItem[ItemsCount];
                if (Ground is not null)
                {
                    items[currentIndex] = Ground;
                    currentIndex++;
                }
                
                TopItems?.CopyTo(items,currentIndex);

                currentIndex += TopItems?.Count ?? 0;

               
                DownItems?.CopyTo(items, currentIndex);
                
                return items;
            }
        }
        
        public bool HasCreature => (Creatures?.Count ?? 0) > 0;

        public List<IPlayer> Players
        {
            get
            {
                if (Creatures is null) return new List<IPlayer>(0);
                var players = new List<IPlayer>(Creatures.Count);
                foreach (var (_,walkableCreature) in Creatures)
                {
                    if(walkableCreature is not IPlayer player) continue;
                    players.Add(player);
                }

                return players;
            }
        }

        public bool HasTeleport(out ITeleport teleport)
        {
            teleport = null;
            if (TopItems is null) return false;

            foreach (var topItem in TopItems)
            {
                if (topItem is ITeleport teleportItem)
                {
                    teleport = teleportItem;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Get the top item on DownItems's stack
        /// </summary>
        public override IItem TopItemOnStack => DownItems != null && DownItems.TryPeek(out var item) ? item :
            TopItems is not null && TopItems.TryPeek(out item) ? item : Ground;

        public IMagicField MagicField
        {
            get
            {
                if (!HasFlag(TileFlags.MagicField)) return null;

                foreach (var downItem in DownItems)
                    if (downItem is IMagicField magicField)
                        return magicField;

                RemoveFlag(TileFlags.MagicField);
                return null;
            }
        }

        public ICreature GetTopVisibleCreature(ICreature creature)
        {
            if (Creatures is not null)
                foreach (var tileCreature in Creatures.Values)
                    //var tileCreature = Creatures[creatureId];
                    if (creature != null)
                    {
                        if (creature.CanSee(tileCreature)) return tileCreature;
                    }
                    else
                    {
                        var isPlayer = tileCreature is IPlayer;

                        var player = isPlayer ? tileCreature as IPlayer : null;

                        if (!tileCreature.IsInvisible)
                            if (!isPlayer || !player.IsInvisible)
                                return tileCreature;
                    }

            return null;
        }

        public override bool TryGetStackPositionOfThing(IPlayer observer, IThing thing, out byte stackPosition)
        {
            stackPosition = default;

            if (thing is IItem item)
                return TryGetStackPositionOfItem(observer, item, out stackPosition);
            if (thing is ICreature creature)
                return TryGetStackPositionOfCreature(observer, creature, out stackPosition);

            return false;
        }

        public bool TryGetStackPositionOfItem(IItem item, out byte stackPosition)
        {
            stackPosition = 0;

            var id = item.ClientId;
            if (id == default) throw new ArgumentNullException(nameof(id));

            if (Ground?.ClientId == id) return true;
            if (Ground?.ClientId != 0) ++stackPosition;

            if (item.IsAlwaysOnTop)
            {
                foreach (var topItem in TopItems)
                    if (id == topItem.ClientId)
                        return true;
                    else if (++stackPosition == 10) return false;
            }
            else
            {
                stackPosition += (byte)(TopItems?.Count ?? 0);
                if (stackPosition >= 10) return false;
            }

            if (!item.IsAlwaysOnTop && DownItems is not null)
                foreach (var downItem in DownItems)
                    if (id == downItem.ClientId)
                        return true;
                    else if (++stackPosition >= 10) return false;

            return false;
        }

        public override byte GetCreatureStackPositionIndex(IPlayer observer)
        {
            if (Creatures is null) return 0;

            byte stackPosition = 0;
            foreach (var creature in Creatures)
                if (observer.CanSee(creature.Value) && ++stackPosition >= 10)
                    return 0;
            return stackPosition;
        }

        public bool HasBlockPathFinding => HasFlag(TileFlags.BlockPath);
        
        public byte[] GetRaw(IPlayer playerRequesting)
        {
            if (_cache != null && !(Creatures?.Any() ?? false)) return _cache;

            Span<byte> stream = stackalloc byte[930]; //max possible length

            var countThings = 0;
            var countBytes = 0;

            if (Ground != default)
            {
                BitConverter.GetBytes(Ground.ClientId).AsSpan().CopyTo(stream);

                countThings++;
                countBytes += 2;
            }

            if (TopItems is not null)
                foreach (var item in TopItems.Reverse()) //todo: remove reverse
                {
                    if (countThings == 9) break;

                    var raw = item.GetRaw();
                    raw.CopyTo(stream.Slice(countBytes, raw.Length));

                    countThings++;
                    countBytes += raw.Length;
                }

            if (Creatures is not null)
                foreach (var creature in Creatures)
                {
                    if (!playerRequesting.CanSee(creature.Value)) continue;
                    if (countThings == 9) break;

                    var raw = creature.Value.GetRaw(playerRequesting);
                    playerRequesting.AddKnownCreature(creature.Key);

                    raw.CopyTo(stream.Slice(countBytes, raw.Length));

                    countThings++;
                    countBytes += raw.Length;
                }

            if (DownItems is not null)
                foreach (var item in DownItems)
                {
                    if (countThings == 9) break;

                    var raw = item.GetRaw();
                    raw.CopyTo(stream.Slice(countBytes, raw.Length));

                    countThings++;
                    countBytes += raw.Length;
                }

            _cache = stream.Slice(0, countBytes).ToArray();
            return _cache;
        }

        private bool TryGetStackPositionOfItem(IPlayer observer, IItem item, out byte stackPosition)
        {
            TryGetStackPositionOfItem(item, out stackPosition);

            if (stackPosition >= 10) return false;

            stackPosition = (byte)(stackPosition +
                                   (item.IsAlwaysOnTop || item is IGround
                                       ? 0
                                       : GetCreatureStackPositionIndex(observer)));

            return false;
        }

        internal void ReplaceGround(IGround ground)
        {
            AddItem(ground);
        }

        public event AddCreatureToTile CreatureAdded;

        private bool TryGetStackPositionOfCreature(IPlayer observer, ICreature creature, out byte stackPosition)
        {
            stackPosition = default;

            var id = creature.CreatureId;
            if (id == default) throw new ArgumentNullException(nameof(id));

            if (Ground?.ClientId != 0) stackPosition++;

            if (TopItems is not null)
            {
                stackPosition += (byte)TopItems.Count;
                if (stackPosition >= 10) return false;
            }

            if (Creatures is not null)
                foreach (var c in Creatures.Values?.Reverse())
                    if (c == creature)
                        return true;
                    else if (observer.CanSee(creature))
                        if (++stackPosition >= 10)
                            return false;
            return false;
        }

        private void SetGround(IGround ground)
        {
            var operations = new OperationResult<IItem>();

            if (Ground is not null) operations.Add(Operation.Updated, ground);
            if (Ground is null) operations.Add(Operation.Added, ground);

            Ground = ground;
            FloorDirection = ground.FloorDirection;
            
            TileOperationEvent.OnChanged(this, ground, operations);
        }

        public Result<OperationResult<ICreature>> AddCreature(ICreature creature)
        {
            if (creature is not IWalkableCreature walkableCreature)
                return Result<OperationResult<ICreature>>.NotPossible;
            if (HasCreature) return Result<OperationResult<ICreature>>.NotPossible;

            Creatures ??= new Dictionary<uint, IWalkableCreature>();
            Creatures.TryAdd(creature.CreatureId, walkableCreature);

            walkableCreature.SetCurrentTile(this);

            SetCacheAsExpired();

            CreatureAdded?.Invoke(walkableCreature, this);
            Ground?.CreatureEntered(walkableCreature);

            return new Result<OperationResult<ICreature>>(new OperationResult<ICreature>(Operation.Added, creature));
        }

        private OperationResult<IItem> AddItemToTile(IItem item)
        {
            var operations = new OperationResult<IItem>();
            
            if (Guard.IsNull(item)) return operations;

            if (item is IGround ground)
            {
                SetGround(ground);
            }
            else
            {
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

                    if (!DownItems.TryPeek(out var topStackItem))
                    {
                        DownItems.Push(item);
                        operations.Add(Operation.Added, item);
                    }
                    else if (item is ICumulative cumulative &&
                             topStackItem is ICumulative topCumulative &&
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

                    if (item.Metadata.Attributes.HasAttribute(ItemAttribute.Field)) SetFlag(TileFlags.MagicField);
                }
            }

            SetTileFlags(item);

            SetCacheAsExpired();
            return operations;
        }

        private void AddContent(IGround ground, IItem[] topItems, IItem[] items)
        {
            if (topItems?.Length > 0) TopItems = new Stack<IItem>(10);
            if (items?.Length > 0) DownItems = new Stack<IItem>(10);

            if (ground != null)
            {
                Ground = ground;
                SetTileFlags(ground);
            }

            if (topItems is not null)
                foreach (var item in topItems)
                {
                    TopItems.Push(item);
                    SetTileFlags(item);
                }

            if (items is not null)
                foreach (var item in items)
                {
                    AddItem(item);
                    SetTileFlags(item);
                }
        }

        private void SetCacheAsExpired()
        {
            _cache = null;
        }

        public Result<OperationResult<ICreature>> RemoveCreature(ICreature creature, out ICreature removedCreature)
        {
            Creatures ??= new Dictionary<uint, IWalkableCreature>();

            Creatures.Remove(creature.CreatureId, out var c);
            removedCreature = c;
            SetCacheAsExpired();

            return new Result<OperationResult<ICreature>>(new OperationResult<ICreature>(Operation.Removed, creature));
        }

        public IItem[] RemoveAllItems()
        {
            if (DownItems is null) return Array.Empty<IItem>();

            var removedItems = new IItem[DownItems.Count];

            var i = 0;
            while (DownItems.TryPeek(out var topItem))
            {
                RemoveItem(topItem, topItem.Amount, 0, out var removedItem);
                removedItems[i++] = removedItem;
            }

            return removedItems;
        }

        public ICreature[] RemoveAllCreatures()
        {
            if (Creatures is null) return Array.Empty<ICreature>();

            var removedCreatures = new ICreature[Creatures.Count];

            var i = 0;
            while (Creatures.Any())
            {
                var creature = Creatures.First().Value;
                RemoveCreature(creature, out var removedCreature);
                removedCreatures[i++] = removedCreature;
            }

            return removedCreatures;
        }

        public Result<OperationResult<IItem>> RemoveItem(IItem itemToRemove, byte amount, out IItem removedItem)
        {
            var operations = new OperationResult<IItem>();

            TryGetStackPositionOfItem(itemToRemove, out var stackPosition);
            removedItem = null;

            if (itemToRemove.IsAlwaysOnTop)
            {
                TopItems.TryPop(out var item);
                operations.Add(Operation.Removed, item, stackPosition);
                removedItem = item;
            }
            else if (DownItems is not null && DownItems.TryPeek(out var topStackItem))
            {
                if (itemToRemove is ICumulative && topStackItem is ICumulative topCumulative)
                {
                    var amountBeforeSplit = topCumulative.Amount;
                    removedItem = topCumulative.Split(amount);

                    if ((removedItem?.Amount ?? 0) == amountBeforeSplit)
                    {
                        DownItems.TryPop(out var item);
                        operations.Add(Operation.Removed, item, stackPosition);
                    }
                    else
                    {
                        operations.Add(Operation.Updated, topCumulative);
                    }
                }
                else
                {
                    DownItems.TryPop(out var item);
                    operations.Add(Operation.Removed, item, stackPosition);
                    removedItem = item;
                }
            }
            else if (itemToRemove == Ground)
            {
                Ground = null;
                operations.Add(Operation.Removed, itemToRemove, stackPosition);
                removedItem = itemToRemove;
            }

            SetCacheAsExpired();
            
            ResetTileFlags(AllItems);
            
            TileOperationEvent.OnChanged(this, itemToRemove, operations);
            return new Result<OperationResult<IItem>>(operations);
        }

        public bool RemoveTopItem(out IItem removedItem)
        {
            removedItem = null;
            
            if (TopItemOnStack is not IMoveableThing) return false;
            RemoveItem(TopItemOnStack, TopItemOnStack.Amount, out removedItem);
            
            return true;
        }

        #region Store Methods

        public override Result CanAddItem(IItem thing, byte amount = 1, byte? slot = null)
        {
            if (HasFlag(TileFlags.Unpassable))  return new Result(InvalidOperation.NotEnoughRoom);
            
            if (thing is null) return new Result(InvalidOperation.NotPossible);
            
            if (thing is IGround) return Result.Success;

            if (thing is { IsAlwaysOnTop: true } && TopItems?.Count >= 10)
                return new Result(InvalidOperation.NotEnoughRoom);

            if (thing is { IsAlwaysOnTop: false } && DownItems?.Count >= 10)
                return new Result(InvalidOperation.NotEnoughRoom);

            return Result.Success;
        }

        public override bool CanRemoveItem(IItem thing)
        {
            if (thing is { CanBeMoved: false }) return false;

            return true;
        }

        internal void AddItems(IItem[] items)
        {
            foreach (var item in items)
            {
                AddItem(item);
            }
        }

        public override uint PossibleAmountToAdd(IItem thing, byte? toPosition = null)
        {
            var freeSpace = 10 - (DownItems?.Count ?? 0);
            if (thing is not ICumulative cumulative)
            {
                if (freeSpace <= 0) return 0;
                return (uint)freeSpace;
            }

            var possibleAmountToAdd = freeSpace * 100;
            if (TopItemOnStack is ICumulative c && TopItemOnStack.ClientId == cumulative.ClientId)
                possibleAmountToAdd += c.AmountToComplete;

            return (uint)possibleAmountToAdd;
        }

        public override Result<OperationResult<IItem>> RemoveItem(IItem thing, byte amount, byte fromPosition,
            out IItem removedThing)
        {
            amount = amount == 0 ? (byte)1 : amount;
            var result = RemoveItem(thing, amount, out removedThing);
            return result;
        }

        public override Result<OperationResult<IItem>> AddItem(IItem thing, byte? position = null)
        {
            var operations = AddItemToTile(thing);
            if (operations.HasAnyOperation) thing.Location = Location;
            if (thing is IContainer container) container.SetParent(null);

            TileOperationEvent.OnChanged(this, thing, operations);
            return new Result<OperationResult<IItem>>(operations);
        }

        public override Result<uint> CanAddItem(IItemType itemType)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}