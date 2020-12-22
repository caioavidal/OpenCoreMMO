using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Location.Structs.Helpers;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Common;
using NeoServer.Game.Contracts.World.Tiles;

namespace NeoServer.Game.World.Map.Tiles
{
    public class StaticTile : BaseTile, IStaticTile
    {
        public StaticTile(Coordinate coordinate, params IItem[] items)
        {
            Location = new Location((ushort)coordinate.X, (ushort)coordinate.Y, (byte)coordinate.Z);
            Raw = null;
            Raw = GetRaw(items);
        }

        public byte[] Raw { get; }

        public FloorChangeDirection FloorDirection { get; private set; }
        private IItem _topItemOnStack;
        public override IItem TopItemOnStack => _topItemOnStack;
        public override ICreature TopCreatureOnStack => null;

        public byte[] GetRaw(IItem[] items)
        {

            var ground = new List<byte>();
            var top1 = new List<byte>();
            var downRawItems = new List<byte>();

            foreach (var item in items)
            {
                if (item is IGround groundItem)
                {
                    _topItemOnStack = groundItem;
                    ground.AddRange(BitConverter.GetBytes(item.ClientId));
                    continue;
                }

                if (item.IsAlwaysOnTop)
                {
                    if (item.FloorDirection != default)
                    {
                        FloorDirection = item.FloorDirection;
                    }

                    _topItemOnStack = item;
                    top1.AddRange(BitConverter.GetBytes(item.ClientId));
                }
                else
                {
                    _topItemOnStack = item;
                    downRawItems.AddRange(BitConverter.GetBytes(item.ClientId));
                }
            }

            return ground.Concat(top1).Concat(downRawItems).ToArray();

        }

        public override int GetHashCode()
        {
            return HashHelper.Start
                .CombineHashCode(Raw);
        }

        public override bool TryGetStackPositionOfThing(IPlayer player, IThing thing, out byte stackPosition)
        {
            stackPosition = default;
            return false;
        }
        public override byte GetCreatureStackPositionCount(IPlayer observer) => 0;

        #region Store Methods
        public override Result CanAddThing(IThing item, byte amount=1, byte? slot = null) => new Result(InvalidOperation.NotEnoughRoom);
        public override bool CanRemoveItem(IThing item) => false;
        public override int PossibleAmountToAdd(IThing item, byte? toPosition = null) => 0;
        public override Result<OperationResult<IThing>> RemoveThing(IThing thing, byte amount, byte fromPosition, out IThing removedThing)
        {
            removedThing = null;
            return Result<OperationResult<IThing>>.NotPossible;
        }
        public override Result<OperationResult<IThing>> AddThing(IThing thing, byte? position = null) => Result<OperationResult<IThing>>.NotPossible;
        #endregion

    }
}
