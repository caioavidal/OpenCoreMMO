using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Services;
using NeoServer.Game.Item.Services.ItemTransform;

namespace NeoServer.Application.Features.Decay;

public interface IItemDecayTracker
{
    void Track(IItem item);
    void DecayExpiredItems();
}

public class ItemDecayTracker : IItemDecayTracker
{
    private readonly IItemTransformService _itemTransformService;
    private PriorityQueue<IItem, DateTime> Items { get; } = new();

    public ItemDecayTracker(IItemTransformService itemTransformService) => _itemTransformService = itemTransformService;

    public void Track(IItem item)
    {
        if (item?.Decay is null || item.Decay.Expired) return;

        var expiresAt = DateTime.Now.AddSeconds(item.Decay.Remaining);
        Items.Enqueue(item, expiresAt);
    }

    public void DecayExpiredItems()
    {
        while (Items.Count > 0)
        {
            var item = Items.Peek();

            if (!item.IsDeleted && !item.Decay.IsPaused && !item.Decay.Expired) break;

            if (item.Decay.Expired) ProcessDecay(item);

            Items.Dequeue();
        }
    }

    private void ProcessDecay(IItem item)
    {
        if (item.Decay is null) return;

        item.Decay.TryDecay();

        var transformedItem = _itemTransformService.Transform(item, item.Decay.DecaysTo);

        if (!transformedItem.Succeeded) return;

        HandleTransformedItem(transformedItem.Value);
    }

    private void HandleTransformedItem(IItem item)
    {
        item?.Decay?.StartDecay();
        Track(item);
    }
}