using NeoServer.Server.Contracts.Repositories;
using NeoServer.Server.Model;
using System;

namespace NeoServer.Server.Handlers.Authentication
{
    public class AccountLoginEventHandler: IEventHandler
    {
        private readonly IAccountRepository _repository;

        public AccountLoginEventHandler(IAccountRepository repository)
        {
            _repository = repository;
        }

        public void OnIncomingMessage(object sender, ServerEventArgs args)
        {
            var account = args.Model as Account;
            Console.WriteLine("login");
        }
    }
}
