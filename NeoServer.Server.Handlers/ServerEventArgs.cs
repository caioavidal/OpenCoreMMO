using System;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model;

namespace NeoServer.Server.Handlers
{
    public class ServerEventArgs
    {
        public ServerEventArgs(IServerModel model, IConnection connection, Func<Account, IOutgoingPacket> successFunc)
        
        {
            Model = model;
            SuccessFunc = successFunc;
            Connection = connection;
        }
        public IServerModel Model { get; }
        public IConnection Connection { get;}
        public Func<Account, IOutgoingPacket> SuccessFunc { get; }
    }
}