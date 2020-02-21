using NeoServer.Networking.Protocols;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Networking.Listeners
{
    public class LoginListener : OpenTibiaListener
    {
        public LoginListener(LoginProtocol protocol) : base(7171, protocol) //todo: remover instancia daqui
        {

        }
    }
}
