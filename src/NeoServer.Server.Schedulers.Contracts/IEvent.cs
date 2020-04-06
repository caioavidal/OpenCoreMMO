namespace NeoServer.Server.Schedulers.Contracts
{
    public interface IEvent
    {
        string EventId { get; }
        uint RequestorId { get; }
        string ErrorMessage { get; }
    }
}