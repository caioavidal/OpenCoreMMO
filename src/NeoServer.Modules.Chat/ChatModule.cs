using Mediator;
using NeoServer.BuildingBlocks.Application;
using NeoServer.BuildingBlocks.Infrastructure.Threading.Dispatcher;

namespace NeoServer.Modules.Chat;

public class ChatModule: Module, IChatModule
{
    public ChatModule(IDispatcher dispatcher, IMediator mediator) : base(dispatcher, mediator)
    {
    }
}

public interface IChatModule:IModule
{
}