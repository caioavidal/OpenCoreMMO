using System;
using NeoServer.Server.Model;

namespace NeoServer.Server.Handlers
{
    public class ServerEventArgs
    {
        public ServerEventArgs(IServerModel model, Func<Account, object> sendMessageFunc)
        {
            Model = model;
            SendMessageFunc = sendMessageFunc;
        }
        public IServerModel Model { get; }
        public Func<Account, object> SendMessageFunc { get; }
    }
}