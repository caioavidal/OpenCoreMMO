using NeoServer.Networking.Protocols;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Networking.Listeners
{
    public class LoginListener : OpenTibiaListener
    {
        public LoginListener() : base(7171, new LoginProtocol()) //todo: remover instancia daqui
        {

        }
    }
}
