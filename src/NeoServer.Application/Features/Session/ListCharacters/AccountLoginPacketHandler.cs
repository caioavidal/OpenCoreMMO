using NeoServer.Application.Common.PacketHandler;
using NeoServer.Application.Server;
using NeoServer.Infrastructure.Data.Interfaces;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Networking.Packets.Outgoing.Login;
using NeoServer.Server.Common.Contracts.Network;
using Serilog;

namespace NeoServer.Application.Features.Session.ListCharacters;

public class AccountLoginPacketHandler : PacketHandler
{
    private readonly IAccountRepository _accountRepository;
    private readonly ILogger _logger;
    private readonly ServerConfiguration _serverConfiguration;

    public AccountLoginPacketHandler(IAccountRepository accountRepository, ServerConfiguration serverConfiguration,
        ILogger logger)
    {
        _accountRepository = accountRepository;
        _serverConfiguration = serverConfiguration;
        _logger = logger;
    }

    public override async void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        var accountPacket = new AccountLoginPacket(message);

        connection.SetXtea(accountPacket.Xtea);

        if (accountPacket.ProtocolVersion != _serverConfiguration.Version)
        {
            _logger.Warning("Client protocol version {ProtocolVersion} is not supported",
                accountPacket.ProtocolVersion);
            connection.Disconnect("Client protocol not supported");
        }

        connection.SetXtea(accountPacket.Xtea);

        if (!accountPacket.IsValid())
        {
            connection.Disconnect("Invalid account name or password.");
            return;
        }

        var foundedAccount = await _accountRepository.GetAccount(accountPacket.Account, accountPacket.Password);

        if (foundedAccount == null)
        {
            connection.Disconnect("Account name or password is not correct.");
            return;
        }

        if (foundedAccount.BanishedAt is not null)
        {
            connection.Disconnect("Your account has been banished. Reason: " + foundedAccount.BanishmentReason);
            return;
        }

        connection.Send(new CharacterListPacket(foundedAccount, _serverConfiguration.ServerName,
            _serverConfiguration.ServerIp));
    }
}