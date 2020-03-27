using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Server.Contracts.Network
{

    public interface IConnection
    {
        IReadOnlyNetworkMessage InMessage { get; }
        uint[] XteaKey { get; }
        uint PlayerId { get; set; }
        bool IsAuthenticated { get; set; }
        bool Disconnected { get; }

        event EventHandler<IConnectionEventArgs> OnProcessEvent;
        event EventHandler<IConnectionEventArgs> OnCloseEvent;
        event EventHandler<IConnectionEventArgs> OnPostProcessEvent;

        void BeginStreamRead();
        void Close();
        void Disconnect(string text);
        void OnAccept(IAsyncResult ar);
        void ResetBuffer();
        void Send(IOutgoingPacket packet, bool encrypt = true);
        void Send(Queue<IOutgoingPacket> outgoingPackets);
        void SendFirstConnection();
        void SetXtea(uint[] xtea);
    }
}
