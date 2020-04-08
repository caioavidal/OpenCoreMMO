using NeoServer.Networking.Protocols;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Networking.Listeners
{
    public class GameListener : Listener
    {
        public GameListener(GameProtocol protocol) : base(7172, protocol)
        {
        }
    }
}
