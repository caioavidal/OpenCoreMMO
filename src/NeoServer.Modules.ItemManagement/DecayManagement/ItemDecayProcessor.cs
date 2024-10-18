using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Services;

namespace NeoServer.Modules.ItemManagement.DecayManagement;

public interface IItemDecayProcessor
{
    void Decay(IItem item);
    void Decay(List<IItem> items);
}

public class ItemDecayProcessor(IItemTransformService itemTransformService) : IItemDecayProcessor
{
    public void Decay(IItem item)
    {
        if (item.Decay is null) return;

        if (!item.Decay.TryDecay()) return;

        itemTransformService.Transform(item, item.Decay.DecaysTo);
    }

    public void Decay(List<IItem> items)
    {
        foreach (var item in items) Decay(item);
    }
}