using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Server.Model.Authentication
{
    public class Account
    {
        public Account(string accountName, string password)
        {
            AccountName = accountName;
            Password = password;
        }

        public string AccountName { get; }
        public string Password { get; }


    }
}
