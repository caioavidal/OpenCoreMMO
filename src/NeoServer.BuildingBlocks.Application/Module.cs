using Mediator;
using NeoServer.BuildingBlocks.Infrastructure.Threading.Dispatcher;
using NeoServer.BuildingBlocks.Infrastructure.Threading.Event;

namespace NeoServer.BuildingBlocks.Application;

public interface IModule
{
   void ExecuteCommandAsync<TResult>(ICommand<TResult> command, int expiration = 0);

    void ExecuteCommandAsync(ICommand command, int expiration = 0);
}

public abstract class Module(IDispatcher dispatcher, IMediator mediator) : IModule
{
    public void ExecuteCommandAsync<TResult>(ICommand<TResult> command, int expiration = 0)
    {
        dispatcher.AddEvent(new Event(expiration, () => _ = ValueTask.FromResult(mediator.Send(command))));
    }

    public void ExecuteCommandAsync(ICommand command, int expiration = 0)
    {
        dispatcher.AddEvent(new Event(expiration, () => _ = ValueTask.FromResult(mediator.Send(command))));
    }
}