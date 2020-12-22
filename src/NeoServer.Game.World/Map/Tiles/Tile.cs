using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Contracts.World.Tiles;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using NeoServer.Game.Contracts;

namespace NeoServer.Game.World.Map.Tiles
{
    public class Tile : BaseTile, IDynamicTile
    {
        public Tile(Coordinate coordinate, TileFlag tileFlag, IGround ground, IItem[] topItems, IItem[] items)
        {
            Location = new Location((ushort)coordinate.X, (ushort)coordinate.Y, (byte)coordinate.Z);
            flags |= (byte)tileFlag;
            AddContent(ground, topItems, items);
        }

        private byte[] cache;

        public Location Location { get; }
        public ushort StepSpeed => Ground.StepSpeed;

        public override ICreature TopCreatureOnStack => Creatures.FirstOrDefault().Value;

        public FloorChangeDirection FloorDirection { get; private set; } = FloorChangeDirection.None;

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

        public bool TryGetStackPositionOfItem(IItem item, out byte stackPosition)
        {
            stackPosition = 0;

            var id = item.ClientId;
            if (id == default) throw new ArgumentNullException(nameof(id));

            if (Ground?.ClientId == id)
            {
                return true;
            }
            if (Ground?.ClientId != 0)
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
        public override byte GetCreatureStackPositionCount(IPlayer observer)
        {
            byte stackPosition = 0;
            foreach (var creature in Creatures)
            {
                if (observer.CanSee(creature.Value) && ++stackPosition >= 10) return 0;
            }
            return stackPosition;
        }
        private bool TryGetStackPositionOfItem(IPlayer observer, IItem item, out byte stackPosition)
        {
            TryGetStackPositionOfItem(item, out stackPosition);

            if (stackPosition >= 10) return false;

            stackPosition = (byte)(stackPosition + ((item.IsAlwaysOnTop || item is IGround) ? 0 : GetCreatureStackPositionCount(observer)));

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

            if (Ground?.ClientId != 0)
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

        private void SetGround(IGround ground)
        {
            Ground = ground;
            FloorDirection = ground.FloorDirection;
        }

        private OperationResult<IThing> AddThingToTile(IThing thing)
        {
            var operations = new OperationResult<IThing>();

            if (thing is IGround ground && Ground is null)
            {
                SetGround(ground);
                operations.Add(Operation.Added, ground);
            }
            else if (thing is IWalkableCreature creature)
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
                }
            }

            SetCacheAsExpired();
            return operations;
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
                if (FloorDirection == FloorChangeDirection.None) FloorDirection = item.IsUsable ? FloorChangeDirection.None : item.FloorDirection;
                TopItems.Push(item);
            }

            foreach (var item in items)
            {
                AddThing(item, null);
            }
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
                if (creature.Value is IMonster monster && !playerRequesting.CanSee(monster)) continue;
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

        #region Store Methods
        public override Result CanAddThing(IThing thing, byte amount = 1, byte? slot = null)
        {
            if (thing is null) return new Result(InvalidOperation.NotPossible);
            if (thing is IGround) return Result.Success;

            if (thing is IPlayer && Creatures?.Count >= 10) return new Result(InvalidOperation.NotEnoughRoom);
            if (thing is IItem item && item.IsAlwaysOnTop && TopItems?.Count >= 10) return new Result(InvalidOperation.NotEnoughRoom);

            if (thing is IItem down && !down.IsAlwaysOnTop && DownItems?.Count >= 10) return new Result(InvalidOperation.NotEnoughRoom);

            return Result.Success;
        }

        public override bool CanRemoveItem(IThing thing)
        {
            if (thing is IItem item && !item.CanBeMoved) return false;
            if (thing is ICreature) return false; //todo should be configurable

            return true;
        }

        public override int PossibleAmountToAdd(IThing thing, byte? toPosition = null)
        {
            if (thing is ICreature && HasCreature) return 0; //todo: must be configurable

            var freeSpace = 10 - (DownItems?.Count ?? 0);
            if (thing is not ICumulative cumulative)
            {
                if (freeSpace <= 0) return 0;
                return freeSpace;
            }

            var possibleAmountToAdd = freeSpace * 100;
            if (TopItemOnStack is ICumulative c && TopItemOnStack.ClientId == cumulative.ClientId) possibleAmountToAdd += c.AmountToComplete;

            return possibleAmountToAdd;
        }

        public override Result<OperationResult<IThing>> RemoveThing(IThing thing, byte amount, byte fromPosition, out IThing removedThing)
        {
            amount = amount == 0 ? 1 : amount;
            var operations = new OperationResult<IThing>();

            removedThing = null;
            var itemToRemove = thing as IItem;


            if (thing is ICreature c)
            {
                Creatures.Remove(c.CreatureId, out var creature);
                operations.Add(Operation.Removed, creature);
                removedThing = creature;
            }
            else if(thing is IItem itemToBeRemoved)
            {
                TryGetStackPositionOfItem(itemToBeRemoved, out var stackPosition);

                if (itemToRemove.IsAlwaysOnTop)
                {
                    TopItems.TryPop(out var item);
                    operations.Add(Operation.Removed, item, stackPosition);
                    removedThing = item;
                }
                else if (DownItems is not null && DownItems.TryPeek(out var topStackItem))
                {
                    if (thing is ICumulative && topStackItem is ICumulative topCumulative)
                    {
                        var amountBeforeSplit = topCumulative.Amount;
                        removedThing = topCumulative.Split(amount);

                        if ((removedThing?.Amount ?? 0) == amountBeforeSplit)
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
                        removedThing = item;
                    }
                }
                else if (thing == Ground)
                {
                    Ground = null;
                    operations.Add(Operation.Removed, thing, stackPosition);
                    removedThing = thing;
                }
            }

            SetCacheAsExpired();
            TileOperationEvent.OnChanged(this, thing, operations);
            return new(operations);
        }

        public override Result<OperationResult<IThing>> AddThing(IThing thing, byte? position = null)
        {
            var operations = AddThingToTile(thing);
            if (operations.HasAnyOperation) thing.Location = Location;
            if (thing is IContainer container) container.SetParent(null);

            TileOperationEvent.OnChanged(this, thing, operations);
            return new(operations);
        }

        #endregion

    }
}