using Moq;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Tests;
using NeoServer.Game.Tests.Helpers;
using NeoServer.Game.World.Map;
using NeoServer.Networking;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Contracts.Tasks;
using NeoServer.Server.Events;
using NeoServer.Server.Instances;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Xunit;

namespace NeoServer.Server.Tests.Events
{
    public class CreatureMovedEventHandlerTest
    {
        delegate void SubmitCallback(out IConnection connection);

        [Fact]
        public void Execute_When_Spectator_Cant_See_Old_But_New_Should_Send_Corret_Packets()
        {
            var map = MapTestDataBuilder.CreateMap(1, 101, 1, 101, 6, 9);
            var dispatcherMock = new Mock<IDispatcher>();
            var schedulerMock = new Mock<IScheduler>();
            var creatureGameInstanceMock = new Mock<ICreatureGameInstance>();

            var connectionMock = new Mock<IConnection>();
            connectionMock.Setup(x => x.OutgoingPackets).Returns(new Queue<IOutgoingPacket>());
            var connection = connectionMock.Object;
            

            var gameCreatureManagerMock = new Mock<GameCreatureManager>(creatureGameInstanceMock.Object, map);
            gameCreatureManagerMock.Setup(x => x.GetPlayerConnection(It.IsAny<uint>(), out connection)).Returns(true);

            var decayableItemManagerMock = new Mock<DecayableItemManager>();

            var game = new Game(map, dispatcherMock.Object, schedulerMock.Object, gameCreatureManagerMock.Object, decayableItemManagerMock.Object);

            var sup = new CreatureMovedEventHandler(game);
            var player = PlayerTestDataBuilder.BuildPlayer();
            player.SetNewLocation(new Location(50, 50, 7));

            var spec = PlayerTestDataBuilder.BuildPlayer();
            spec.SetNewLocation(new Location(40, 50, 7));

            var spectators = new ICylinderSpectator[] { new CylinderSpectator(spec, 1, 1) };

            var cylinder = new Cylinder(player, map[50, 50, 7], map[49, 50, 7], NeoServer.Game.Common.Operation.Moved, spectators);

            sup.Execute(player, cylinder);
        }
    }
}
