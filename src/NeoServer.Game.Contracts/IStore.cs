using NeoServer.Game.Common;
using NeoServer.Game.Contracts.Items;

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
        Result CanAddItem(IItem item,byte amount = 1, byte? slot = null);
        /// <summary>
        /// Gives amount that can be added to store
        /// </summary>
        /// <param name="thing"></param>
        /// <returns></returns>
        int PossibleAmountToAdd(IItem thing, byte? toPosition = null);
        /// <summary>
        /// Checks if thing can be removed from store
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool CanRemoveItem(IItem item);

        /// <summary>
        /// Stores thing
        /// </summary>
        /// <param name="thing">thing to be stored</param>
        /// <param name="position">position where thing will be stored</param>
        /// <returns></returns>
        Result<OperationResult<IItem>> AddItem(IItem thing, byte? position = null);
        /// <summary>
        /// Removes thing from store
        /// </summary>
        /// <param name="thing">thing to be removed</param>
        /// <param name="amount">amount of the thing to be removed</param>
        /// <param name="fromPosition">position where thing will be removed</param>
        /// <param name="removedThing">removed thing instance from store</param>
        /// <returns></returns>
        Result<OperationResult<IItem>> RemoveItem(IItem thing, byte amount, byte fromPosition, out IItem removedThing);

        /// <summary>
        /// Sends thing to another store
        /// </summary>
        /// <param name="destination">store destination</param>
        /// <param name="thing">thing to send</param>
        /// <param name="fromPosition">position source in store</param>
        /// <param name="toPosition">position destination in store</param>
        /// <returns></returns>
        Result<OperationResult<IItem>> SendTo(IStore destination, IItem thing, byte amount, byte fromPosition, byte? toPosition);

        /// <summary>
        /// Receives thing from another store
        /// </summary>
        /// <param name="source">source store where thing is coming</param>
        /// <param name="thing">thing being received</param>
        /// <param name="toPosition">destination position where thing will be stored</param>
        /// <returns></returns>
        Result<OperationResult<IItem>> ReceiveFrom(IStore source, IItem thing, byte? toPosition);

    }
}
