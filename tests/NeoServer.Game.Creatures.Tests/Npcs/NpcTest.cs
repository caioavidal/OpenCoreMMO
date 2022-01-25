using System.Threading;
using Moq;
using NeoServer.Game.Common.Chats;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Creatures.Npcs;
using NeoServer.Game.Tests.Helpers;
using Xunit;

namespace NeoServer.Game.Creatures.Tests.Npcs;

public class NpcTest
{
    private readonly Mock<IOutfit> outfit = new();
    private readonly Mock<ISpawnPoint> spawnPoint = new();

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
        //arrange
        var npcType = new Mock<INpcType>();

        var name = "Eryn";

        npcType.Setup(x => x.Name).Returns(name);
        npcType.Setup(x => x.Dialogs).Returns(new IDialog[]
        {
            new Dialog
            {
                OnWords = new[] { "hi" },
                StoreAt = "greetings",
                Action = "ok"
            }
        });

        var sut = NpcTestDataBuilder.Build(name, npcType.Object);
        var creature = new Mock<ISociableCreature>();

        creature.SetupGet(x => x.CreatureId).Returns(1);

        sut.Hear(creature.Object, SpeechType.PrivatePlayerToNpc, "hi");

        //act
        var result = sut.GetPlayerStoredValues(creature.Object);

        //assert
        Assert.Single(result);
        Assert.Equal("hi", result["greetings"]);
    }

    [Fact]
    public void GetPlayerKeywordsHistory_When_Sends_Multiple_Words_Returns_Them()
    {
        var npcType = new Mock<INpcType>();

        npcType.Setup(x => x.Name).Returns("Eryn");
        npcType.Setup(x => x.Dialogs).Returns(new IDialog[]
        {
            new Dialog
            {
                OnWords = new[] { "hi" },
                StoreAt = "greetings",
                Then = new IDialog[]
                {
                    new Dialog
                    {
                        OnWords = new[] { "trade" },
                        StoreAt = "trade",
                        Then = new IDialog[]
                        {
                            new Dialog
                            {
                                StoreAt = "answer",
                                OnWords = new[] { "ok" }
                            }
                        }
                    }
                }
            }
        });

        var sut = NpcTestDataBuilder.Build("Eryn", npcType.Object);
        var creature = new Mock<ISociableCreature>();

        creature.SetupGet(x => x.CreatureId).Returns(1);

        sut.Hear(creature.Object, SpeechType.PrivatePlayerToNpc, "hi");
        sut.Hear(creature.Object, SpeechType.PrivatePlayerToNpc, "blablabla");
        sut.Hear(creature.Object, SpeechType.PrivatePlayerToNpc, "trade");
        sut.Hear(creature.Object, SpeechType.PrivatePlayerToNpc, "blablabla");
        sut.Hear(creature.Object, SpeechType.PrivatePlayerToNpc, "ok");

        var result = sut.GetPlayerStoredValues(creature.Object);
        Assert.Equal(3, result.Count);
        Assert.Equal("hi", result["greetings"]);
        Assert.Equal("trade", result["trade"]);
        Assert.Equal("ok", result["answer"]);
    }

    [Fact]
    public void SendMessageTo_When_Has_Bind_Variables_Should_Replace_It()
    {
        //arrange
        var npcType = new Mock<INpcType>();

        npcType.Setup(x => x.Name).Returns("Eryn");
        npcType.Setup(x => x.Dialogs).Returns(new IDialog[]
        {
            new Dialog
            {
                OnWords = new[] { "hi" },
                Answers = new[] { "what city?" },
                Then = new IDialog[]
                {
                    new Dialog
                    {
                        OnWords = new[] { "carlin", "thais" },
                        StoreAt = "city",
                        Answers = new[] { "ok, you said {{city}}." }
                    }
                }
            }
        });

        var anwser = "";

        var sut = NpcTestDataBuilder.Build("Eryn", npcType.Object);

        sut.ReplaceKeywords = (m, _, _) => m;
        var creature = new Mock<IPlayer>();

        sut.OnSay += (_, _, message, _) => { anwser = message; };

        creature.SetupGet(x => x.CreatureId).Returns(1);

        sut.Hear(creature.Object, SpeechType.PrivatePlayerToNpc, "hi");
        sut.Hear(creature.Object, SpeechType.PrivatePlayerToNpc, "carlin");

        Assert.Equal("ok, you said carlin.", anwser);
    }

    [Fact]
    public void Advertise_Should_Call_Say_With_Message()
    {
        var npcType = new Mock<INpcType>();

        npcType.Setup(x => x.Name).Returns("Eryn");
        npcType.Setup(x => x.Marketings).Returns(new[] { "this is a advertise" });

        var advertise = "";
        var speechType = SpeechType.None;

        var sut = NpcTestDataBuilder.Build("Eryn", npcType.Object);

        sut.OnSay += (_, b, message, _) =>
        {
            advertise = message;
            speechType = b;
        };

        Thread.Sleep(10_000); //todo: try remove this
        sut.Advertise();

        Assert.Equal("this is a advertise", advertise);
        Assert.Equal(SpeechType.Say, speechType);
    }

    [Fact]
    public void WalkRandomStep_Should_Emit_OnStartedWalking()
    {
        //arrange
        var npcType = new Mock<INpcType>();

        npcType.Setup(x => x.Name).Returns("Eryn");
        npcType.Setup(x => x.Speed).Returns(200);

        var startedWalking = false;

        var sut = NpcTestDataBuilder.Build("Eryn", npcType.Object);

        sut.OnStartedWalking += _ => startedWalking = true;

        Thread.Sleep(5_000); //todo: try remove this

        //act
        var result = sut.WalkRandomStep();

        //assert
        Assert.True(startedWalking);
        Assert.True(result);
    }

    [Fact]
    public void OnCustomerLeft_Should_Emit_When_Player_Logout()
    {
        var npcType = new Mock<INpcType>();

        npcType.Setup(x => x.Name).Returns("Eryn");
        npcType.Setup(x => x.Speed).Returns(200);
        npcType.Setup(x => x.Dialogs).Returns(new IDialog[] { new Dialog { OnWords = new[] { "hi" } } });

        var customer = new Mock<IPlayer>();
        customer.Setup(x => x.CreatureId).Returns(123);
        customer.Setup(x => x.Location).Returns(new Location(100, 101, 7));

        var npcTile = new Mock<IDynamicTile>();
        npcTile.Setup(x => x.Location).Returns(new Location(100, 100, 7));

        var eventCalled = false;

        var sut = NpcTestDataBuilder.Build("Eryn", npcType.Object);
        sut.OnCustomerLeft += _ => eventCalled = true;

        sut.SetCurrentTile(npcTile.Object);

        sut.Hear(customer.Object, SpeechType.PrivatePlayerToNpc, "hi");

        customer.Object.Logout();
        customer.Raise(f => f.OnLoggedOut += null, customer.Object);

        Assert.True(eventCalled);
    }
}