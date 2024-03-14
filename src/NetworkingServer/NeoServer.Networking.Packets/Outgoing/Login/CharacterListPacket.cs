using System.Collections.Generic;
using NeoServer.Networking.Packets.Network;

namespace NeoServer.Networking.Packets.Outgoing.Login;

public class CharacterListPacket : OutgoingPacket
{
    private readonly Account _account;
    private readonly string _ipAddress;

    private readonly string _serverName;

    public CharacterListPacket(Account account, string serverName, string ipAddress)
    {
        _serverName = serverName;
        _ipAddress = ipAddress;
        _account = account;
    }

    public override void WriteToMessage(INetworkMessage message)
    {
        AddCharList(message);
    }

    private void AddCharList(INetworkMessage message)
    {
        message.AddByte(0x64); //todo charlist
        message.AddByte((byte)_account.Players.Count);

        var ipAddress = ParseIpAddress(_ipAddress);

        foreach (var player in _account.Players)
        {
            if (!string.IsNullOrWhiteSpace(player.WorldIp)) ipAddress = ParseIpAddress(player.WorldIp);

            message.AddString(player.Name);
            message.AddString(player.WorldName ?? _serverName ?? string.Empty);

            message.AddByte(ipAddress[0]);
            message.AddByte(ipAddress[1]);
            message.AddByte(ipAddress[2]);
            message.AddByte(ipAddress[3]);

            message.AddUInt16(7172);
        }

        message.AddUInt16((ushort)_account.PremiumTime);
    }

    private static byte[] ParseIpAddress(string ip)
    {
        var localhost = new byte[] { 127, 0, 0, 1 };

        if (string.IsNullOrEmpty(ip)) return localhost;

        var parsedIp = new byte[4];

        var numbers = ip.Split(".");

        if (numbers.Length != 4) return localhost;

        var i = 0;

        foreach (var number in numbers)
        {
            if (!byte.TryParse(numbers[i], out var ipNumber)) return localhost;
            parsedIp[i++] = ipNumber;
        }

        return parsedIp;
    }


    public record Account(List<Player> Players, int PremiumTime);

    public record Player(string WorldIp, string WorldName, string Name);
}