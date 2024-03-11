using NeoServer.Application.Common.Contracts;
using NeoServer.Application.Features.Item.Decay;
using NeoServer.Application.Infrastructure.Thread;

namespace NeoServer.Application.Server.Jobs.Items;

public class DecayRoutine(IGameServer game, IItemDecayTracker itemDecayTracker, IItemDecayProcessor itemDecayProcessor)
{
    private const ushort EVENT_CHECK_ITEM_INTERVAL = 1000;

    public void StartChecking()
    {
        game.Scheduler.AddEvent(new SchedulerEvent(EVENT_CHECK_ITEM_INTERVAL, StartChecking));

        var expiredItems = itemDecayTracker.GetExpiredItems();

        itemDecayProcessor.Decay(expiredItems);
    }
}