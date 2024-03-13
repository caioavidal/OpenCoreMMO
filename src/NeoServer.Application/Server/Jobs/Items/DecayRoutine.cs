using NeoServer.Application.Common.Contracts;
using NeoServer.Application.Features.Item.Decay;
using NeoServer.Application.Infrastructure.Thread;
using Serilog;

namespace NeoServer.Application.Server.Jobs.Items;

public class DecayRoutine(
    IGameServer game,
    IItemDecayTracker itemDecayTracker,
    IItemDecayProcessor itemDecayProcessor,
    ILogger logger)
{
    private const ushort EVENT_CHECK_ITEM_INTERVAL = 1000;
    private const ushort LOG_INTERVAL_SECONDS = 20;

    private DateTime _lastLogMessageTime = DateTime.MinValue;

    public void StartChecking()
    {
        game.Scheduler.AddEvent(new SchedulerEvent(EVENT_CHECK_ITEM_INTERVAL, StartChecking));

        var expiredItems = itemDecayTracker.GetExpiredItems();

        itemDecayProcessor.Decay(expiredItems);

        var elapsedSecondsSinceLastLog = (DateTime.Now - _lastLogMessageTime).Seconds;

        if (elapsedSecondsSinceLastLog < LOG_INTERVAL_SECONDS && expiredItems.Count == 0) return;

        logger.Debug("{NumberOfItems} item(s) to decay and {Expired} expired",
            itemDecayTracker.GetCountOfItemsToDecay(),
            expiredItems.Count);

        _lastLogMessageTime = DateTime.Now;
    }
}