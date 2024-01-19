using System.Threading;
using Moq;
using NeoServer.Application.Features.Chat.PlayerSay;
using NeoServer.Game.Common.Chats;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Infrastructure.InMemory;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;
using Xunit;

namespace NeoServer.Server.Tests.Commands;

public class PlayerSayCommandTest
{
    [Fact]
    public void Execute_When_Player_Send_Message_To_Another_Player_Should_Call_Player_Method_Once()
    {
        //arrange
        var player = new Mock<IPlayer>();
        var connection = new Mock<IConnection>();
        var network = new Mock<IReadOnlyNetworkMessage>();

        var playerSayPacket = new Mock<PlayerSayPacket>(network.Object);
        playerSayPacket.SetupGet(x => x.TalkType).Returns(SpeechType.Private);
        playerSayPacket.SetupGet(x => x.Receiver).Returns("receiver");
        playerSayPacket.SetupGet(x => x.Message).Returns("hello");

        var chatChannelStore = new ChatChannelStore();

        var receiverMock = new Mock<IPlayer>();
        var receiver = receiverMock.Object;

        var game = new Mock<IGameServer>();
        game.Setup(x => x.CreatureManager.TryGetPlayer("receiver", out receiver)).Returns(true);

        var playerSayCommand = new PlayerSayCommand(player.Object, connection.Object, SpeechType.Private,"receiver", "hello", 1);

        var sut = new PlayerSayCommandHandler(game.Object.Map, chatChannelStore, game.Object.CreatureManager);

        //act
        sut.Handle(playerSayCommand, new CancellationToken());

        //assert
        player.Verify(x => x.SendMessageTo(receiver, SpeechType.Private, "hello"), Times.Once());
    }
}