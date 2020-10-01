using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;

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
        long LastPingRequest { get; set; }
        long LastPingResponse { get; set; }
        string IP { get; }

        event EventHandler<IConnectionEventArgs> OnProcessEvent;
        event EventHandler<IConnectionEventArgs> OnCloseEvent;
        event EventHandler<IConnectionEventArgs> OnPostProcessEvent;

        void BeginStreamRead();
        void Close(bool force = false);
        void Disconnect(string text);
        void Send(IOutgoingPacket packet);
        void SendFirstConnection();
        void SetXtea(uint[] xtea);
        void SetAsAuthenticated();
        void SetConnectionOwner(IPlayer player);
        void Send();
    }
}
