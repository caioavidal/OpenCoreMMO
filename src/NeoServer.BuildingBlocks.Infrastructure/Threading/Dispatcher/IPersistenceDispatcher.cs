namespace NeoServer.BuildingBlocks.Infrastructure.Threading.Dispatcher;

public interface IPersistenceDispatcher
{
    void AddEvent(Func<Task> evt);

    void Start(CancellationToken token);
}