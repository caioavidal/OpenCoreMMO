using NeoServer.Server.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Server.Handlers
{
    public interface IEventHandler
    {
        void Handler(object sender, IServerModel model);
    }
}
