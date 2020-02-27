using System;
using NeoServer.Server.Model;

namespace NeoServer.Server.Handlers
{
    public class ServerEventArgs
    {
        public ServerEventArgs(IServerModel model)
        {
            Model = model;
        }
        public IServerModel Model { get; }
        public Func<Account, object> OutputFunc{ get;}
    }
}