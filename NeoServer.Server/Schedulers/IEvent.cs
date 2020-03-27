namespace NeoServer.Server.Schedulers
{
    public interface IEvent
    {
        string EventId { get; }
        uint RequestorId { get; }
        string ErrorMessage { get; }
        void Execute();
    }
}