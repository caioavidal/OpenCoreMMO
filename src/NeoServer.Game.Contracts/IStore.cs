using NeoServer.Game.Common;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Contracts.World.Tiles;

namespace NeoServer.Game.Contracts
{
    /// <summary>
    /// A contract to represent anything that can store things
    /// </summary>
    public interface IStore
    {
        /// <summary>
        /// Checks if thing can be added to store
        /// </summary>
        /// <param name="item"></param>
        /// <param name="slot"></param>
        /// <returns></returns>
        Result CanAddThing(IThing item,byte amount = 1, byte? slot = null);
        /// <summary>
        /// Gives amount that can be added to store
        /// </summary>
        /// <param name="thing"></param>
        /// <returns></returns>
        int PossibleAmountToAdd(IThing thing, byte? toPosition = null);
        /// <summary>
        /// Checks if thing can be removed from store
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool CanRemoveItem(IThing item);

        /// <summary>
        /// Stores thing
        /// </summary>
        /// <param name="thing">thing to be stored</param>
        /// <param name="position">position where thing will be stored</param>
        /// <returns></returns>
        Result<OperationResult<IThing>> AddThing(IThing thing, byte? position = null);
        /// <summary>
        /// Removes thing from store
        /// </summary>
        /// <param name="thing">thing to be removed</param>
        /// <param name="amount">amount of the thing to be removed</param>
        /// <param name="fromPosition">position where thing will be removed</param>
        /// <param name="removedThing">removed thing instance from store</param>
        /// <returns></returns>
        Result<OperationResult<IThing>> RemoveThing(IThing thing, byte amount, byte fromPosition, out IThing removedThing);

        /// <summary>
        /// Sends thing to another store
        /// </summary>
        /// <param name="destination">store destination</param>
        /// <param name="thing">thing to send</param>
        /// <param name="fromPosition">position source in store</param>
        /// <param name="toPosition">position destination in store</param>
        /// <returns></returns>
        public Result SendTo(IStore destination, IThing thing, byte amount, byte fromPosition, byte? toPosition)
        {
            var canAdd = destination.CanAddThing(thing, amount, toPosition);
            if (!canAdd.IsSuccess) return canAdd;

            var possibleAmountToAdd = destination.PossibleAmountToAdd(thing,toPosition);
            if (possibleAmountToAdd == 0) return new Result(InvalidOperation.NotEnoughRoom);

            IThing removedThing;
            if (thing is not ICumulative cumulative)
            {
                if (possibleAmountToAdd < 1) return new Result(InvalidOperation.NotEnoughRoom);
                RemoveThing(thing, 1, fromPosition, out removedThing);

            }
            else
            {
                var amountToAdd = (byte)(possibleAmountToAdd < amount ? possibleAmountToAdd : amount);

                RemoveThing(thing, amountToAdd, fromPosition, out removedThing);
            }

            var result = destination.ReceiveFrom(this, removedThing, toPosition);

            if (amount - possibleAmountToAdd > 0)
                return SendTo(destination, thing, (byte)(amount - possibleAmountToAdd), fromPosition, toPosition);

            return result;
        }

        /// <summary>
        /// Receives thing from another store
        /// </summary>
        /// <param name="source">source store where thing is coming</param>
        /// <param name="thing">thing being received</param>
        /// <param name="toPosition">destination position where thing will be stored</param>
        /// <returns></returns>
        public Result ReceiveFrom(IStore source, IThing thing, byte? toPosition)
        {
            var canAdd = CanAddThing(thing);
            if (!canAdd.IsSuccess) return canAdd;

            return AddThing(thing, toPosition).ResultValue;
        }

    }
}
