using NeoServer.Game.Common.Contracts.Items;

namespace NeoServer.Application.Features.Item.Decay;

public class DecayItemSubscriber(IItemDecayTracker itemDecayTracker) : IItemEventSubscriber
{
    public void Subscribe(IItem item)
    {
        if (item.Decay is null) return;

        item.Decay.OnStarted += itemDecayTracker.Track;
    }

    public void Unsubscribe(IItem item)
    {
        item.Decay.OnStarted -= itemDecayTracker.Track;
    }
}