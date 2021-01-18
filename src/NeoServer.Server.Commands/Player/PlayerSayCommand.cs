using NeoServer.Networking.Packets.Incoming;
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

        public PlayerSayCommand(IPlayer player, PlayerSayPacket playerSayPacket, Game game)
        {
            this.player = player;
            this.playerSayPacket = playerSayPacket;
            this.game = game;
        }

        public override void Execute()
        {
            switch (playerSayPacket.Talk)
            {
                case NeoServer.Game.Common.Talks.TalkType.None:
                    break;
                case NeoServer.Game.Common.Talks.TalkType.Say:
                    break;
                case NeoServer.Game.Common.Talks.TalkType.Whisper:
                    break;
                case NeoServer.Game.Common.Talks.TalkType.Yell:
                    break;
                case NeoServer.Game.Common.Talks.TalkType.PrivatePn:
                    break;
                case NeoServer.Game.Common.Talks.TalkType.PrivateNp:
                    break;
                case NeoServer.Game.Common.Talks.TalkType.Private:
                    break;
                case NeoServer.Game.Common.Talks.TalkType.ChannelY:
                    break;
                case NeoServer.Game.Common.Talks.TalkType.ChannelW:
                    break;
                case NeoServer.Game.Common.Talks.TalkType.RvrChannel:
                    break;
                case NeoServer.Game.Common.Talks.TalkType.RvrAnswer:
                    break;
                case NeoServer.Game.Common.Talks.TalkType.RvrContinue:
                    break;
                case NeoServer.Game.Common.Talks.TalkType.Broadcast:
                    break;
                case NeoServer.Game.Common.Talks.TalkType.ChannelR1:
                    break;
                case NeoServer.Game.Common.Talks.TalkType.PrivateRed:
                    break;
                case NeoServer.Game.Common.Talks.TalkType.ChannelO:
                    break;
                case NeoServer.Game.Common.Talks.TalkType.ChannelR2:
                    break;
                case NeoServer.Game.Common.Talks.TalkType.MonsterSay:
                    break;
                case NeoServer.Game.Common.Talks.TalkType.MonsterYell:
                    break;
                default:
                    break;
            }
        }
    }
}
