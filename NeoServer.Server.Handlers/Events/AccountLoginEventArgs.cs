using NeoServer.Server.Model.Authentication;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Server.Handlers.Events
{
    public class AccountLoginEventArgs : EventArgs
    {
        public AccountLoginEventArgs(Account account)
        {
            Account = account;
        }

        public Account Account { get; }


    }
}
