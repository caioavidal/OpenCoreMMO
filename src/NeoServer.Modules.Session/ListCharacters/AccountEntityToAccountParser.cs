using NeoServer.Data.Entities;
using NeoServer.Networking.Packets.Outgoing.Login;

namespace NeoServer.Modules.Session.ListCharacters;

public static class AccountEntityToAccountParser
{
    public static CharacterListPacket.Account ParseToAccount(this AccountEntity accountEntity)
    {
        var players = accountEntity
            .Players
            .Select(x =>
                new CharacterListPacket.Player(x.World.Ip, x.World.Name, x.Name))
            .ToList();

        return new CharacterListPacket.Account(players, accountEntity.PremiumTime);
    }
}