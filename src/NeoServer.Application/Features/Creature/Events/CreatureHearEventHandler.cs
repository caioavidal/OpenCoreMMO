using NeoServer.Game.Common.Chats;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Networking.Packets.Outgoing.Creature;
using NeoServer.Server.Common.Contracts;

namespace NeoServer.Application.Features.Creature.Events;

public class CreatureHearEventHandler : IEventHandler
{
    private readonly IGameServer _game;

    public CreatureHearEventHandler(IGameServer game)
    {
        _game = game;
    }

    public void Execute(ICreature from, ISociableCreature receiver, SpeechType type, string message)
    {
        if (from is null || receiver is null || type == SpeechType.None || string.IsNullOrEmpty(message)) return;

        SendMessage(from, receiver, type, message);
    }

    private void SendMessage(ICreature from, ICreature to, SpeechType type, string message)
    {
        if (!_game.CreatureManager.GetPlayerConnection(to.CreatureId, out var connection)) return;

        connection.OutgoingPackets.Enqueue(new CreatureSayPacket(from, type, message));

        connection.Send();
    }
}