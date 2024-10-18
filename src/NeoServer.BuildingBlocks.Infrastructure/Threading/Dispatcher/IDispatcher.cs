using NeoServer.BuildingBlocks.Infrastructure.Threading.Event;

namespace NeoServer.BuildingBlocks.Infrastructure.Threading.Dispatcher;

public interface IDispatcher
{
    void AddEvent(IEvent evt);

    void Start(CancellationToken token);

    /// <summary>
    ///     Adds an event to dispatcher queue
    /// </summary>
    /// <param name="evt"></param>
    void AddEvent(Action evt);
}