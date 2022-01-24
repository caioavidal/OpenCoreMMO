using System.Threading;

namespace NeoServer.Networking.Listeners;

internal interface IListener
{
    void BeginListening(CancellationToken cancellationToken);
    void EndListening();
}