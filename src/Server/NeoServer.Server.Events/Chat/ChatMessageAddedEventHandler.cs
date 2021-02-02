using NeoServer.Game.Common.Talks;
using NeoServer.Game.Contracts.Chats;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Server.Events.Chat
{
    public class ChatMessageAddedEventHandler
    {
        private readonly Game game;

        public ChatMessageAddedEventHandler(Game game)
        {
            this.game = game;
        }

        public void Execute(ISociableCreature player, IChatChannel chatChannel, SpeechType speechType, string message)
        {
            if (chatChannel is null) return;
            if (string.IsNullOrWhiteSpace(message)) return;

            foreach (var user in chatChannel.Users)
            {
                if (!game.CreatureManager.GetPlayerConnection(user.Player.CreatureId, out IConnection connection)) continue;
                connection.OutgoingPackets.Enqueue(new MessageToChannelPacket(player, speechType, message, chatChannel.Id));
                connection.Send();
            }
        }
    }
}
