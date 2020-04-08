using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Server.Contracts.Network
{

    public interface IConnection
    {
        IReadOnlyNetworkMessage InMessage { get; }
        uint[] XteaKey { get; }
        uint PlayerId { get; }
        bool IsAuthenticated { get; }
        bool Disconnected { get; }
        Queue<IOutgoingPacket> OutgoingPackets { get; }

        event EventHandler<IConnectionEventArgs> OnProcessEvent;
        event EventHandler<IConnectionEventArgs> OnCloseEvent;
        event EventHandler<IConnectionEventArgs> OnPostProcessEvent;

        void BeginStreamRead();
        void Close();
        void Disconnect(string text);
        void Send(IOutgoingPacket packet, bool notification = false);
        void Send(Queue<IOutgoingPacket> outgoingPackets, bool notification = false);
        void SendFirstConnection();
        void SetXtea(uint[] xtea);
        void SetAsAuthenticated();
        void SetConnectionOwner(IPlayer player);
        void Send(bool notification = false);
    }
}
