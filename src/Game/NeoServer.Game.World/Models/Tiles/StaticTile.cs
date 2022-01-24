using System;
using System.Collections.Generic;
using System.Linq;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Location.Structs.Helpers;

namespace NeoServer.Game.World.Models.Tiles;

public class StaticTile : BaseTile, IStaticTile
{
    private IItem _topItemOnStack;

    public StaticTile(Coordinate coordinate, params IItem[] items)
    {
        Location = new Location((ushort)coordinate.X, (ushort)coordinate.Y, (byte)coordinate.Z);
        Raw = GetRaw(items);
        ThingsCount = items.Length;
    }

    public override int ThingsCount { get; }
    public byte[] Raw { get; }
    public override IItem TopItemOnStack => _topItemOnStack;
    public override ICreature TopCreatureOnStack => null;

    public override bool TryGetStackPositionOfThing(IPlayer player, IThing thing, out byte stackPosition)
    {
        stackPosition = default;
        return false;
    }

    public override byte GetCreatureStackPositionIndex(IPlayer observer)
    {
        return 0;
    }

    public byte[] GetRaw(IItem[] items)
    {
        var ground = new List<byte>();
        var top1 = new List<byte>();
        var downRawItems = new List<byte>();

        foreach (var item in items)
        {
            if (item is null) continue;

            if (item is IGround groundItem)
            {
                _topItemOnStack = groundItem;
                ground.AddRange(BitConverter.GetBytes(item.ClientId));
                continue;
            }

            if (item.IsAlwaysOnTop)
            {
                if (item.FloorDirection != default) FloorDirection = item.FloorDirection;

                _topItemOnStack = item;
                top1.AddRange(BitConverter.GetBytes(item.ClientId));
            }
            else
            {
                _topItemOnStack = item;
                downRawItems.InsertRange(0, BitConverter.GetBytes(item.ClientId));
            }

            SetTileFlags(item);
        }

        return ground.Concat(top1).Concat(downRawItems).ToArray();
    }

    public override int GetHashCode()
    {
        return HashHelper.START
            .CombineHashCode(Raw);
    }

    #region Store Methods

    public override Result CanAddItem(IItem item, byte amount = 1, byte? slot = null)
    {
        return new Result(InvalidOperation.NotEnoughRoom);
    }

    public override bool CanRemoveItem(IItem item)
    {
        return false;
    }

    public override uint PossibleAmountToAdd(IItem item, byte? toPosition = null)
    {
        return 0;
    }

    public override Result<OperationResult<IItem>> RemoveItem(IItem thing, byte amount, byte fromPosition,
        out IItem removedThing)
    {
        removedThing = null;
        return Result<OperationResult<IItem>>.NotPossible;
    }

    public override Result<OperationResult<IItem>> AddItem(IItem thing, byte? position = null)
    {
        return Result<OperationResult<IItem>>.NotPossible;
    }

    public override Result<uint> CanAddItem(IItemType itemType)
    {
        throw new NotImplementedException();
    }

    #endregion
}