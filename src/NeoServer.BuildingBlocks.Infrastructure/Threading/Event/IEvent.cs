namespace NeoServer.BuildingBlocks.Infrastructure.Threading.Event;

public interface IEvent
{
    Action Action { get; }

    bool HasExpired { get; }
    bool HasNoTimeout { get; }

    void SetToNotExpire();
}