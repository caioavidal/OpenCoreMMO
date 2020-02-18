using NeoServer.Networking.Protocols;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Networking.Listeners
{
    public class GameListener : OpenTibiaListener
    {
        public GameListener() : base(7172, new GameProtocol())
        {
        }
    }
}
