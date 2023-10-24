using Mediator;

namespace NeoServer.Game.Common.Contracts;

public interface IGameEvent : INotification
{
}

public interface IHasEvent
{
    Queue<IGameEvent> Events { get; }
    void RaiseEvent(IGameEvent gameEvent);
}