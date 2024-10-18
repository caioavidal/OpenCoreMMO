using Mediator;
using NeoServer.BuildingBlocks.Application.Contracts;
using NeoServer.BuildingBlocks.Application.Enums;
using NeoServer.BuildingBlocks.Application.Server;
using NeoServer.BuildingBlocks.Infrastructure.Threading.Event;
using NeoServer.Modules.Session.LogIn;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Networking.Packets.Network;
using NeoServer.Networking.Packets.Outgoing;

namespace NeoServer.PacketHandler.Modules.Session.LogIn;

public class PlayerLogInPacketHandler : PacketHandler
{
    private readonly IGameServer _game;
    private readonly IMediator _mediator;
    private readonly ServerConfiguration _serverConfiguration;

    public PlayerLogInPacketHandler(
        IGameServer game, ServerConfiguration serverConfiguration,
        IMediator mediator)
    {
        _game = game;
        _serverConfiguration = serverConfiguration;
        _mediator = mediator;
    }

    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        if (_game.State == GameState.Stopped) connection.Close();

        var packet = new PlayerLogInPacket(message);

        connection.SetXtea(packet.Xtea);

        //todo linux os

        _game.Dispatcher.AddEvent(new Event(() => SendLogInCommand(connection, packet)));
    }

    private void SendLogInCommand(IConnection connection, PlayerLogInPacket packet)
    {
        var logInCommand = new LogInCommand(packet.Account, packet.Password, packet.CharacterName, packet.Version,
            connection);

        var error = _mediator.Send(logInCommand).Result;

        if (error is InvalidLoginOperation.None) return;

        var errorDescription = error.GetDescription();

        if (error is InvalidLoginOperation.ClientVersionNotAllowed)
            errorDescription = errorDescription.Replace("{{version}}", _serverConfiguration.Version.ToString());

        Disconnect(connection, errorDescription);
    }

    private static void Disconnect(IConnection connection, string message)
    {
        connection.Send(new GameServerDisconnectPacket(message));
        connection.Close();
    }
}