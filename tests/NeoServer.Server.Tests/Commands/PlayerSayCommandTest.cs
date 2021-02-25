using Moq;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Commands.Player;
using NeoServer.Server.Contracts;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NeoServer.Server.Tests.Commands
{
    public class PlayerSayCommandTest
    {
        [Fact]
        public void Execute_When_Player_Send_Message_To_Another_Player_Should_Call_Player_Method_Once()
        {
            var player = new Mock<IPlayer>();
            var connection = new Mock<IConnection>();
            var network = new Mock<IReadOnlyNetworkMessage>();

            var playerSayPacket = new Mock<PlayerSayPacket>(network.Object);
            playerSayPacket.SetupGet(x => x.TalkType).Returns(NeoServer.Game.Common.Talks.SpeechType.Private);
            playerSayPacket.SetupGet(x => x.Receiver).Returns("receiver");
            playerSayPacket.SetupGet(x => x.Message).Returns("hello");

            var receiverMock = new Mock<IPlayer>();
            var receiver = receiverMock.Object;

            var game = new Mock<IGameServer>();
            game.Setup(x => x.CreatureManager.TryGetPlayer("receiver", out receiver)).Returns(true);

            var sut = new PlayerSayCommand(game.Object);

            sut.Execute(player.Object, connection.Object, playerSayPacket.Object);

            player.Verify(x => x.SendMessageTo(receiver, NeoServer.Game.Common.Talks.SpeechType.Private, "hello"), Times.Once());
        }
    }
}
