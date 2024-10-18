using NeoServer.Game.Common.Contracts.Items;

namespace NeoServer.Modules.ItemManagement.DecayManagement;

public interface IItemDecayTracker
{
    void Track(IItem item);
    List<IItem> GetExpiredItems();
    int GetCountOfItemsToDecay();
}

public class ItemDecayTracker : IItemDecayTracker
{
    private PriorityQueue<IItem, DateTime> Items { get; } = new();

    /// <summary>
    ///     Add decay item to be tracked
    /// </summary>
    public void Track(IItem item)
    {
        if (item?.Decay is null) return;

        var expiresAt = DateTime.Now.AddSeconds(item.Decay.Remaining);
        Items.Enqueue(item, expiresAt);
    }

    /// <summary>
    ///     Returns Expired Items
    /// </summary>
    public List<IItem> GetExpiredItems()
    {
        var expiredItems = new List<IItem>();

        while (Items.Count > 0)
        {
            var item = Items.Peek();

            if (!item.IsDeleted && !item.Decay.IsPaused && !item.Decay.Expired) break;

            if (item.Decay.Expired) expiredItems.Add(item);

            Items.Dequeue();
        }

        return expiredItems;
    }

    public int GetCountOfItemsToDecay()
    {
        return Items.Count;
    }
}