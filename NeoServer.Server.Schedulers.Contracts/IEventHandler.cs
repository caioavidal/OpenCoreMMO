namespace NeoServer.Server.Schedulers.Contracts
{
    public interface IEventHandler<TEvent> where TEvent : IEvent
    {
        void Execute(TEvent evt);
    }
}