using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Server
{
    public class ServerState
    {
        public ServerStatus Status { get; private set; } = ServerStatus.Closed;

        public void OpenServer() => Status = ServerStatus.Opened;
        public void CloseServer() => Status = ServerStatus.Closed;

    }
}
