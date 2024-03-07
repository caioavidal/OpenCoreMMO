namespace NeoServer.Application.Common.Contracts.Tasks;

public interface IPersistenceDispatcher
{
    void AddEvent(Func<Task> evt);

    void Start(CancellationToken token);
}