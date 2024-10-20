using NeoServer.BuildingBlocks.Application.Contracts;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Networking.Packets.Outgoing;

namespace NeoServer.Modules.Players.Events;

public class InvalidOperationEventHandler : IEventHandler
{
    private readonly IGameServer game;

    public InvalidOperationEventHandler(IGameServer game)
    {
        this.game = game;
    }

    public void Execute(IThing thing, InvalidOperation error)
    {
        if (thing is not IPlayer player) return;
        if (!game.CreatureManager.GetPlayerConnection(player.CreatureId, out var connection)) return;

        connection.OutgoingPackets.Enqueue(new TextMessagePacket(TextMessageOutgoingParser.Parse(error),
            TextMessageOutgoingType.Small));
        connection.Send();
    }
}