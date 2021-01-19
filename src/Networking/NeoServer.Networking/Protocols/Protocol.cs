using NeoServer.Server.Contracts.Network;

namespace NeoServer.Networking.Protocols
{
    public abstract class Protocol : IProtocol
    {
        public virtual bool KeepConnectionOpen { get; protected set; }

        public virtual void OnAccept(IConnection connection)
        {
            connection.BeginStreamRead();

            //todo ip ban validation
        }

        public void PostProcessMessage(object sender, IConnectionEventArgs args)
        {
            if (!KeepConnectionOpen)
            {
                args.Connection.Close();
                return;
            }
        }

        public abstract void ProcessMessage(object sender, IConnectionEventArgs connection);
    }
}
