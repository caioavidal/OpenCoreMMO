using NeoServer.Server.Contracts.Repositories;
using NeoServer.Server.Model;
using System;

namespace NeoServer.Server.Handlers.Authentication
{
    public class AccountLoginEventHandler : IEventHandler
    {
        private readonly IAccountRepository _repository;

        public AccountLoginEventHandler(IAccountRepository repository)
        {
            _repository = repository;
        }

        public void Handler(object sender, IServerModel model)
        {
            Console.WriteLine("login");
        }
    }
}
