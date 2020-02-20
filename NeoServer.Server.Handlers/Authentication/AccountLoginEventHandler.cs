using NeoServer.Server.Handlers.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Server.Handlers.Authentication
{
    public class AccountLoginEventHandler: IEventHandler
    {
        public static AccountLoginEventHandler Instance { get; private set; } = Instance ?? new AccountLoginEventHandler();
        public void Handler(object sender, EventArgs args)
        {
            throw new NotImplementedException();
        }
    }
}
