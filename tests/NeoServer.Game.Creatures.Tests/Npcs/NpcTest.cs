using Moq;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Talks;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Contracts.World.Tiles;
using NeoServer.Game.Creatures.Npcs;
using NeoServer.Game.DataStore;
using System.Threading;
using Xunit;

namespace NeoServer.Game.Creatures.Tests.Npcs
{
    public class NpcTest
    {
        private Mock<IOutfit> outfit = new Mock<IOutfit>();
        private Mock<ISpawnPoint> spawnPoint = new Mock<ISpawnPoint>();

        [Fact]
        public void GetPlayerKeywordsHistory_When_Player_Has_Not_Talked_To_Npc_Returns_Null()
        {
            var sut = new Mock<INpc>();
            var creature = new Mock<ISociableCreature>();
            creature.SetupGet(x => x.CreatureId).Returns(1);
            var result = sut.Object.GetPlayerStoredValues(creature.Object);
            Assert.Null(result);
        }
        [Fact]
        public void GetPlayerKeywordsHistory_When_Sends_Hi_Returns_Hi()
        {
            var npcType = new Mock<INpcType>();

            npcType.Setup(x => x.Name).Returns("Eryn");
            npcType.Setup(x => x.Dialogs).Returns(new IDialog[] { new Dialog()
            {
                OnWords = new string[] { "hi" },
                StoreAt = "greetings",
                Action = "ok"
            } });

            var sut = new Npc(npcType.Object, spawnPoint.Object, outfit.Object, 100);
            var creature = new Mock<ISociableCreature>();

            creature.SetupGet(x => x.CreatureId).Returns(1);

            sut.Hear(creature.Object, Common.Talks.SpeechType.PrivatePlayerToNpc, "hi");

            var result = sut.GetPlayerStoredValues(creature.Object);
            Assert.Single(result);
            Assert.Equal("hi", result["greetings"]);

        }
        [Fact]
        public void GetPlayerKeywordsHistory_When_Sends_Multiple_Words_Returns_Them()
        {
            var npcType = new Mock<INpcType>();

            npcType.Setup(x => x.Name).Returns("Eryn");
            npcType.Setup(x => x.Dialogs).Returns(new IDialog[] { new Dialog()
            {
                OnWords = new string[] { "hi" },
                StoreAt = "greetings",
                Then =new IDialog[] { new Dialog()
                {
                     OnWords = new string[] { "trade" },
                     StoreAt = "trade",
                       Then =new IDialog[] { new Dialog()
                        {
                             StoreAt = "answer",
                             OnWords =new string[] { "ok" },
                        } }
                } }
            } });

            var sut = new Npc(npcType.Object, spawnPoint.Object, outfit.Object, 100);
            var creature = new Mock<ISociableCreature>();

            creature.SetupGet(x => x.CreatureId).Returns(1);

            sut.Hear(creature.Object, Common.Talks.SpeechType.PrivatePlayerToNpc, "hi");
            sut.Hear(creature.Object, Common.Talks.SpeechType.PrivatePlayerToNpc, "blablabla");
            sut.Hear(creature.Object, Common.Talks.SpeechType.PrivatePlayerToNpc, "trade");
            sut.Hear(creature.Object, Common.Talks.SpeechType.PrivatePlayerToNpc, "blablabla");
            sut.Hear(creature.Object, Common.Talks.SpeechType.PrivatePlayerToNpc, "ok");

            var result = sut.GetPlayerStoredValues(creature.Object);
            Assert.Equal(3, result.Count);
            Assert.Equal("hi", result["greetings"]);
            Assert.Equal("trade", result["trade"]);
            Assert.Equal("ok", result["answer"]);
        }

        [Fact]
        public void SendMessageTo_When_Has_Bind_Variables_Should_Replace_It()
        {
            var npcType = new Mock<INpcType>();

            npcType.Setup(x => x.Name).Returns("Eryn");
            npcType.Setup(x => x.Dialogs).Returns(new IDialog[] { new Dialog()
            {
                OnWords = new string[] { "hi" },
                Answers = new string[]{ "what city?" },
                Then =new IDialog[] { new Dialog()
                {
                     OnWords = new string[] { "carlin","thais" },
                     StoreAt = "city",
                     Answers = new string[]{ "ok, you said {{city}}." }
                } }
            } });

            var anwser = "";

            var sut = new Npc(npcType.Object, spawnPoint.Object, outfit.Object, 100);

            sut.ReplaceKeywords = (m, a, b) => m;
            var creature = new Mock<IPlayer>();

            sut.OnSay += (a, b, message, d) =>
            {
                anwser = message;
            };

            creature.SetupGet(x => x.CreatureId).Returns(1);

            sut.Hear(creature.Object, Common.Talks.SpeechType.PrivatePlayerToNpc, "hi");
            sut.Hear(creature.Object, Common.Talks.SpeechType.PrivatePlayerToNpc, "carlin");

            Assert.Equal("ok, you said carlin.", anwser);
        }

        [Fact]
        public void Advertise_Should_Call_Say_With_Message()
        {
            var npcType = new Mock<INpcType>();

            npcType.Setup(x => x.Name).Returns("Eryn");
            npcType.Setup(x => x.Marketings).Returns(new string[] { "this is a advertise" });

            var advertise = "";
            var speechType = SpeechType.None;

            var sut = new Npc(npcType.Object, spawnPoint.Object, outfit.Object, 100);
            sut.OnSay += (a, b, message, d) =>
            {
                advertise = message;
                speechType = b;
            };

            Thread.Sleep(10_000);//todo: try remove this
            sut.Advertise();

            Assert.Equal("this is a advertise", advertise);
            Assert.Equal(SpeechType.Say, speechType);

        }

        [Fact]
        public void WalkRandomStep_Should_Emit_OnStartedWalking()
        {
            var npcType = new Mock<INpcType>();

            var pathFinder = new Mock<IPathFinder>();
            pathFinder.Setup(x => x.FindRandomStep(It.IsAny<ICreature>(), It.IsAny<ITileEnterRule>())).Returns(Direction.North);
            ConfigurationStore.PathFinder = pathFinder.Object;

            npcType.Setup(x => x.Name).Returns("Eryn");
            npcType.Setup(x => x.Speed).Returns(200);

            var startedWalking = false;

            var sut = new Npc(npcType.Object, spawnPoint.Object, outfit.Object, 100);
            sut.OnStartedWalking += (a) => startedWalking = true;

            Thread.Sleep(5_000);//todo: try remove this
            var result = sut.WalkRandomStep();

            Assert.True(startedWalking);
            Assert.True(result);
        }

        [Fact]
        public void OnCustomerLeft_Should_Emit_When_Player_Logout()
        {
            var npcType = new Mock<INpcType>();

            var pathFinder = new Mock<IPathFinder>();
            pathFinder.Setup(x => x.FindRandomStep(It.IsAny<ICreature>(), It.IsAny<ITileEnterRule>())).Returns(Direction.North);
            ConfigurationStore.PathFinder = pathFinder.Object;

            npcType.Setup(x => x.Name).Returns("Eryn");
            npcType.Setup(x => x.Speed).Returns(200);
            npcType.Setup(x => x.Dialogs).Returns(new IDialog[] { new Dialog() { OnWords = new string[] { "hi" } } });

            var customer = new Mock<IPlayer>();
            customer.Setup(x => x.CreatureId).Returns(123);
            customer.Setup(x => x.Location).Returns(new Location(100, 101, 7));
            
            var npcTile = new Mock<IDynamicTile>();
            npcTile.Setup(x => x.Location).Returns(new Location(100, 100, 7));

            var eventCalled = false;

            var sut = new Npc(npcType.Object, spawnPoint.Object, outfit.Object, 100);
            sut.OnCustomerLeft += (a) => eventCalled = true;

            sut.SetCurrentTile(npcTile.Object);

            sut.Hear(customer.Object, SpeechType.PrivatePlayerToNpc, "hi");

            customer.Object.Logout();
            customer.Raise(f => f.OnLoggedOut += null, customer.Object);

            Assert.True(eventCalled);
        }

    }
}
