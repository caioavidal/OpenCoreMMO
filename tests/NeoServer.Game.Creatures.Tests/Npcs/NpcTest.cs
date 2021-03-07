using Moq;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Creatures.Npcs;
using NeoServer.Server.Model.Players.Contracts;
using Xunit;

namespace NeoServer.Game.Creatures.Tests.Npcs
{
    public class NpcTest
    {
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
            var outfit = new Mock<IOutfit>();
            var pathAccess = new Mock<IPathAccess>();

            npcType.Setup(x => x.Name).Returns("Eryn");
            npcType.Setup(x => x.Dialogs).Returns(new IDialog[] { new Dialog()
            {
                OnWords = new string[] { "hi" },
                StoreAt = "greetings",
                Action = "ok"
            } });

            var sut = new Npc(npcType.Object, pathAccess.Object, outfit.Object, 100);
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
            var outfit = new Mock<IOutfit>();
            var pathAccess = new Mock<IPathAccess>();

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

            var sut = new Npc(npcType.Object, pathAccess.Object, outfit.Object, 100);
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
            var outfit = new Mock<IOutfit>();
            var pathAccess = new Mock<IPathAccess>();

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
          
            var sut = new Npc(npcType.Object, pathAccess.Object, outfit.Object, 100);

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
    }
}
