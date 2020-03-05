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

        public async void OnIncomingMessage(object sender, ServerEventArgs args)
        {
            var account = args.Model as Account;

            if (account == null)
            { //todo: use option
                args.Connection.Send("Invalid account.");
                return;
            }

            if (!account.IsValid())
            {
                args.Connection.Send("Invalid account name or password.");
                return;
            }

            var foundedAccount = await _repository.Get(account.AccountName,
            account.Password);

            if (foundedAccount == null)
            {
                args.Connection.Send("Account name or password is not correct.");
                return;
            }

            args.Connection.Send(args.SuccessFunc.Invoke(foundedAccount));

        }
    }
}
