using NeoServer.Networking.Packets.Incoming;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Server.Commands.Player
{
    public class PlayerSayCommand : Command
    {
        private readonly IPlayer player;
        private readonly PlayerSayPacket playerSayPacket;
        private readonly Game game;
        private readonly IConnection connection;

        public PlayerSayCommand(IPlayer player, IConnection connection, PlayerSayPacket playerSayPacket, Game game)
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
            
            switch (playerSayPacket.Talk)
            {
                case NeoServer.Game.Common.Talks.SpeechType.None:
                    break;
                case NeoServer.Game.Common.Talks.SpeechType.Say:
                    player.Say(playerSayPacket.Message, playerSayPacket.Talk);
                    break;
                case NeoServer.Game.Common.Talks.SpeechType.Whisper:
                    break;
                case NeoServer.Game.Common.Talks.SpeechType.Yell:
                    break;
                case NeoServer.Game.Common.Talks.SpeechType.PrivatePn:
                    break;
                case NeoServer.Game.Common.Talks.SpeechType.PrivateNp:

                
                    break;
                case NeoServer.Game.Common.Talks.SpeechType.ChannelY:
                    break;
                case NeoServer.Game.Common.Talks.SpeechType.ChannelW:
                    break;
                case NeoServer.Game.Common.Talks.SpeechType.RvrChannel:
                    break;
                case NeoServer.Game.Common.Talks.SpeechType.RvrAnswer:
                    break;
                case NeoServer.Game.Common.Talks.SpeechType.RvrContinue:
                    break;
                case NeoServer.Game.Common.Talks.SpeechType.Broadcast:
                    break;
                case NeoServer.Game.Common.Talks.SpeechType.ChannelR1:
                    break;
                    case NeoServer.Game.Common.Talks.SpeechType.Private:
                case NeoServer.Game.Common.Talks.SpeechType.PrivateRed:
#if GAME_FEATURE_RULEVIOLATION
		        case TALKTYPE_RVR_ANSWER:
#endif
                    SendMessageToPlayer(message);


                        break;
                case NeoServer.Game.Common.Talks.SpeechType.ChannelO:
                    break;
                case NeoServer.Game.Common.Talks.SpeechType.ChannelR2:
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

            if (!game.CreatureManager.GetPlayerConnection(receiver.CreatureId, out var receiverConnection)) return;

            receiverConnection.OutgoingPackets.Enqueue(new PlayerSendPrivateMessagePacket(player, playerSayPacket.Talk, message));
            receiverConnection.Send();
        }
    }
}
