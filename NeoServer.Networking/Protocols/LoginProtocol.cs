using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Networking.Protocols
{
    public class LoginProtocol : OpenTibiaProtocol
    {
        public override bool KeepConnectionOpen => true;
        public LoginProtocol()
        {
        }
        public override void ProcessMessage(object sender, ConnectionEventArgs connection)
        {

        }
    }
}
