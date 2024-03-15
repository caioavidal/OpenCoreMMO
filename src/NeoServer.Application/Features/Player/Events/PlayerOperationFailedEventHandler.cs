using NeoServer.Application.Common.Contracts;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Creatures;
using NeoServer.Networking.Packets.Network;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Networking.Packets.Outgoing.Effect;

namespace NeoServer.Application.Features.Player.Events;

public class PlayerOperationFailedEventHandler : IEventHandler
{
    private readonly IGameServer game;

    public PlayerOperationFailedEventHandler(IGameServer game)
    {
        this.game = game;
    }

    public void Execute(uint playerId, string message, EffectT effect = EffectT.None)
    {
        if (!game.CreatureManager.GetPlayerConnection(playerId, out var connection)) return;
        if (!game.CreatureManager.TryGetPlayer(playerId, out var player)) return;
        
        SendEffectIfAny(effect, connection, player);

        connection.OutgoingPackets.Enqueue(new TextMessagePacket(message,
            TextMessageOutgoingType.MESSAGE_STATUS_DEFAULT));
        connection.Send();
    }

    public void Execute(uint playerId, InvalidOperation invalidOperation, EffectT effect = EffectT.None)
    {
        if (!game.CreatureManager.GetPlayerConnection(playerId, out var connection)) return;
        if (!game.CreatureManager.TryGetPlayer(playerId, out var player)) return;

        SendEffectIfAny(effect, connection, player);

        connection.OutgoingPackets.Enqueue(new TextMessagePacket(TextMessageOutgoingParser.Parse(invalidOperation),
            TextMessageOutgoingType.MESSAGE_STATUS_DEFAULT));

        connection.Send();
    }

    private static void SendEffectIfAny(EffectT effect, IConnection connection, IPlayer player)
    {
        if (effect != EffectT.None)
            connection.OutgoingPackets.Enqueue(new MagicEffectPacket(player.Location, effect));
    }
}