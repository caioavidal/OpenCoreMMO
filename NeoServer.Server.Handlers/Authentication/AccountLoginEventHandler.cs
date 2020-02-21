using NeoServer.Server.Model;
using System;

namespace NeoServer.Server.Handlers.Authentication
{
    public class AccountLoginEventHandler: IEventHandler
    {
        public static AccountLoginEventHandler Instance { get; private set; } = Instance ?? new AccountLoginEventHandler();
        public void Handler(object sender, IServerModel account)
        {
            var a = (Account) account;
            
            Console.WriteLine("login");
        }
    }
}
