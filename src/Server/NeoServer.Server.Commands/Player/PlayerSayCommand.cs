using NeoServer.Game.Contracts.Chats;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.DataStore;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;
using System.Linq;

namespace NeoServer.Server.Commands.Player
{
    public class PlayerSayCommand : Command
    {
        private readonly IPlayer player;
        private readonly PlayerSayPacket playerSayPacket;
        private readonly IGameServer game;
        private readonly IConnection connection;

        public PlayerSayCommand(IPlayer player, IConnection connection, PlayerSayPacket playerSayPacket, IGameServer game)
        {
            this.player = player;
            this.playerSayPacket = playerSayPacket;
            this.game = game;
            this.connection = connection;
        }

        public override void Execute()
        {
            if (string.IsNullOrWhiteSpace(playerSayPacket.Message) || (playerSayPacket.Message?.Length ?? 0) > 255) return;
            if ((playerSayPacket.Receiver?.Length ?? 0) > 30) return;

            var message = playerSayPacket.Message.Trim();

            if (player.CastSpell(message)) return;

            switch (playerSayPacket.TalkType)
            {
                case NeoServer.Game.Common.Talks.SpeechType.None:
                    break;
                case NeoServer.Game.Common.Talks.SpeechType.Say:
                    player.Say(playerSayPacket.Message, playerSayPacket.TalkType);
                    break;
                case NeoServer.Game.Common.Talks.SpeechType.Whisper:
                    break;
                case NeoServer.Game.Common.Talks.SpeechType.Yell:
                    break;
                case NeoServer.Game.Common.Talks.SpeechType.PrivatePlayerToNpc:
                    SendMessageToNpc(message);
                    break;
                case NeoServer.Game.Common.Talks.SpeechType.PrivateNpcToPlayer:
                    break;

                case NeoServer.Game.Common.Talks.SpeechType.ChannelOrangeText:
                case NeoServer.Game.Common.Talks.SpeechType.ChannelRed1Text:
                case NeoServer.Game.Common.Talks.SpeechType.ChannelYellowText:
                    SendMessageToChannel(playerSayPacket.ChannelId, message);
                    break;

                case NeoServer.Game.Common.Talks.SpeechType.ChannelRed2Text:
                    break;
                case NeoServer.Game.Common.Talks.SpeechType.ChannelWhiteText:
                    break;
                case NeoServer.Game.Common.Talks.SpeechType.RvrChannel:
                    break;
                case NeoServer.Game.Common.Talks.SpeechType.RvrAnswer:
                    break;
                case NeoServer.Game.Common.Talks.SpeechType.RvrContinue:
                    break;
                case NeoServer.Game.Common.Talks.SpeechType.Broadcast:
                    break;
                case NeoServer.Game.Common.Talks.SpeechType.Private:
                case NeoServer.Game.Common.Talks.SpeechType.PrivateRed:
                    SendMessageToPlayer(message);
                    break;
                case NeoServer.Game.Common.Talks.SpeechType.MonsterSay:
                    break;
                case NeoServer.Game.Common.Talks.SpeechType.MonsterYell:
                    break;
                default:
                    break;
            }
        }
        private void SendMessageToPlayer(string message)
        {
            if (string.IsNullOrWhiteSpace(playerSayPacket.Receiver) || !game.CreatureManager.TryGetPlayer(playerSayPacket.Receiver, out var receiver))
            {
                connection.OutgoingPackets.Enqueue(new TextMessagePacket("A player with this name is not online.", TextMessageOutgoingType.Small));
                connection.Send();
                return;
            }

            player.SendMessageTo(receiver, playerSayPacket.TalkType, message);
        }
        private void SendMessageToNpc(string message)
        {
            foreach (var creature in game.Map.GetCreaturesAtPositionZone(player.Location))
            {
                if(creature is INpc npc)
                {
                    npc.Hear(player, playerSayPacket.TalkType, message);
                    return;
                }
            }
        }
        private void SendMessageToChannel(ushort channelId, string message)
        {
            var channel = ChatChannelStore.Data.Get(channelId);

            if (channel is not IChatChannel) channel = player.PrivateChannels.FirstOrDefault(x => x.Id == channelId);

            if (channel is not IChatChannel) return;

            player.SendMessage(channel, message);
        }
    }
}
