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
        Result CanAddThing(IThing item, byte? slot = null);
        /// <summary>
        /// Gives amount that can be added to store
        /// </summary>
        /// <param name="thing"></param>
        /// <returns></returns>
        int PossibleAmountToAdd(IThing thing);
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
        Result<IOperationResult> StoreThing(IThing thing, byte? position);
        /// <summary>
        /// Removes thing from store
        /// </summary>
        /// <param name="thing">thing to be removed</param>
        /// <param name="amount">amount of the thing to be removed</param>
        /// <param name="fromPosition">position where thing will be removed</param>
        /// <returns></returns>
        Result<IOperationResult> RemoveThing(IThing thing, byte amount, byte fromPosition);

        /// <summary>
        /// Sends thing to another store
        /// </summary>
        /// <param name="destination">store destination</param>
        /// <param name="thing">thing to send</param>
        /// <param name="fromPosition">position source in store</param>
        /// <param name="toPosition">position destination in store</param>
        /// <returns></returns>
        public Result SendTo(IStore destination, IThing thing, byte fromPosition, byte? toPosition)
        {
            var canAdd = destination.CanAddThing(thing);
            if (!canAdd.IsSuccess) return canAdd;

            var possibleAmountToAdd = destination.PossibleAmountToAdd(thing);


            if (thing is not ICumulative cumulative)
            {
                if (possibleAmountToAdd < 1) return new Result(InvalidOperation.NotEnoughRoom);
            }
            else
            {
                var amount = (byte)(possibleAmountToAdd < cumulative.Amount ? possibleAmountToAdd : cumulative.Amount);

                var splitted = cumulative.Split(amount);
                RemoveThing(thing, amount, fromPosition);

                return destination.ReceiveFrom(this, splitted, toPosition);
            }

            return destination.ReceiveFrom(this, thing, toPosition);
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

            return StoreThing(thing, toPosition).ResultValue;
        }

    }
}
