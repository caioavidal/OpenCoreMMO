// using System;
// using NeoServer.Game.Common;
// using NeoServer.Game.Common.Contracts.Items;
// using NeoServer.Game.Common.Contracts.Items.Types;
// using NeoServer.Game.Common.Contracts.Items.Types.Containers;
//
// namespace NeoServer.Game.Items.Items.Containers.Container.Operations;
//
// public class AddToContainerOperation
// {
//     private Result AddItem(IContainer toContainer, IItem item, byte slot)
//     {
//         if (item is null) return Result.NotPossible;
//         if (toContainer.Capacity <= slot) throw new ArgumentOutOfRangeException("Slot is greater than capacity");
//
//         if (item is not ICumulative cumulativeItem) return AddItemToFront(item);
//
//         var itemToJoinSlot = GetSlotOfFirstItemNotFully(cumulativeItem);
//
//         if (itemToJoinSlot >= 0 && cumulativeItem is { } cumulative)
//             return TryJoinCumulativeItems(cumulative, (byte)itemToJoinSlot);
//
//         return AddItemToFront(cumulativeItem);
//     }
//     
//     private Result AddItemToFront(IContainer toContainer, IItem item)
//     {
//         if (item is null) return Result.NotPossible;
//         if (toContainer.SlotsUsed >= toContainer.Capacity) return new Result(InvalidOperation.IsFull);
//         item.Location = toContainer.Location;
//         toContainer.Items.Insert(0, item);
//         toContainer.SlotsUsed++;
//
//         if (item is IContainer container) container.SetParent(this);
//
//         UpdateItemsLocation();
//
//         if (item is ICumulative cumulative) cumulative.OnReduced += OnItemReduced;
//
//         OnItemAdded?.Invoke(item, this);
//         return Result.Success;
//     }
// }