using NeoServer.Networking;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Networking.Packets.Messages;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Contracts.Repositories;
using NeoServer.Server.Model;
using System;
using System.Linq;

namespace NeoServer.Server.Handlers.Authentication
{
    public class AccountLoginEventHandler : PacketHandler
    {
        private readonly IAccountRepository _repository;

        public AccountLoginEventHandler(IAccountRepository repository)
        {
            _repository = repository;
        }

        public async override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            var account = new AccountLoginPacket(message);

            connection.SetXtea(account.Xtea);

            if (account == null)
            { //todo: use option
                connection.Disconnect("Invalid account.");
                return;
            }

            if (!account.IsValid())
            {
                connection.Disconnect("Invalid account name or password."); //todo: use gameserverdisconnect
                return;
            }

            var foundedAccount = await _repository.Get(account.Account,
            account.Password);

            if (foundedAccount == null)
            {
                connection.Disconnect("Account name or password is not correct.");
                return;
            }

            connection.Send(new CharacterListPacket(foundedAccount));
        }
    }
}
